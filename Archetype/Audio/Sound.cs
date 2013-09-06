using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrKlang;

namespace Archetype.Audio
{
	public abstract class Sound
	{
		protected ISound ISound
		{
			get { return _iSound; }
			set
			{
				lock (Lock)
				{
					_iSound = value;
					if (Stopped)
						_iSound.Stop();
					else
						_iSound.Paused = Paused;
				}
			}
		}
		public bool Paused
		{
			get { return _paused; }
			set
			{
				lock (Lock)
				{
					_paused = value;
					if (_iSound != null)
						_iSound.Paused = _paused;
				}
			}
		}

		private ISound _iSound;
		private bool _paused = false;
		private readonly object Lock = new object();
		private bool Stopped = false;

		public void Stop()
		{
			lock (Lock)
			{
				Stopped = true;
				if (ISound != null)
					ISound.Stop();
			}
		}
	}
}
