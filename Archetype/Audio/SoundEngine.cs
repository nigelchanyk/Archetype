using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IrrKlang;
using Mogre;

using Archetype.Utilities;

namespace Archetype.Audio
{
	public class SoundEngine : IDisposable
	{
		private bool Exit = false;
		private ISoundEngine ISoundEngine = new ISoundEngine();
		private BlockingCollection<Task> TaskQueue = new BlockingCollection<Task>();
		private Thread Thread;

		public SoundEngine()
		{
			ISoundEngine.SetRolloffFactor(0.5f);
			ISoundEngine.Default3DSoundMinDistance = 5;
			ISoundEngine.Default3DSoundMaxDistance = 50;
			Thread = new Thread(new ThreadStart(ExecuteTask));
			Thread.Start();
		}

		public void Dispose()
		{
			TaskQueue.Add(new DisposeTask(this));
		}

		public Sound Play2D(string fileName)
		{
			MutableSound sound = new MutableSound();
			TaskQueue.Add(new Play2DTask(this, fileName, sound));
			return sound;
		}

		public Sound Play3D(string fileName, Vector3 position)
		{
			MutableSound sound = new MutableSound();
			TaskQueue.Add(new Play3DTask(this, fileName, sound, position));
			return sound;
		}

		public void SetListenerPosition(Vector3 position, Vector3 direction)
		{
			TaskQueue.Add(new SetListenerPositionTask(this, position, direction));
		}

		private void ExecuteTask()
		{
			while (!Exit)
				TaskQueue.Take().Execute();
		}


		private abstract class Task
		{
			protected SoundEngine SoundEngine { get; private set; }
			
			public Task(SoundEngine soundEngine)
			{
				this.SoundEngine = soundEngine;
			}

			public abstract void Execute();
		}


		private class DisposeTask : Task
		{
			public DisposeTask(SoundEngine soundEngine)
				: base(soundEngine)
			{
			}

			public override void Execute()
			{
				SoundEngine.Exit = true;
			}
		}


		private class SetListenerPositionTask : Task
		{
			public Vector3 Direction { get; private set; }
			public Vector3 Position { get; private set; }

			public SetListenerPositionTask(SoundEngine soundEngine, Vector3 position, Vector3 direction)
				: base(soundEngine)
			{
				this.Direction = direction;
				this.Position = position;
			}

			public override void Execute()
			{
				SoundEngine.ISoundEngine.SetListenerPosition(Position, Direction);
			}
		}


		private class Play2DTask : Task
		{
			public string FileName { get; private set; }
			public MutableSound Sound { get; private set; }

			public Play2DTask(SoundEngine soundEngine, string fileName, MutableSound sound)
				: base(soundEngine)
			{
				this.FileName = fileName;
				this.Sound = sound;
			}

			public override void Execute()
			{
				Sound.ISound = SoundEngine.ISoundEngine.Play2D(FileName);
			}
		}


		private class Play3DTask : Play2DTask
		{
			public Vector3 Position { get; private set; }

			public Play3DTask(SoundEngine soundEngine, string fileName, MutableSound sound, Vector3 position)
				: base(soundEngine, fileName, sound)
			{
				this.Position = position;
			}

			public override void Execute()
			{
				Sound.ISound = SoundEngine.ISoundEngine.Play3D(FileName, Position);
			}
		}


		private class MutableSound : Sound
		{
			public new ISound ISound
			{
				get { return base.ISound; }
				set { base.ISound = value; }
			}
		}
	}
}
