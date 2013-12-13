using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            SchedulePushState(new MainMenuState(this));
        }

    }
}
