using Mogre;
using Archetype.States;

namespace Archetype.Applications
{
    public class Game : Application
    {
        public Game() : base("Archetype")
        {
        }

        protected override void Initialize()
        {
            SchedulePushState(new GameConfigurationState(this));
        }

    }
}
