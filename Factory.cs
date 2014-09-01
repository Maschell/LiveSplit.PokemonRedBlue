using LiveSplit.Model;
using LiveSplit.PokemonRedBlue;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(Factory))]

namespace LiveSplit.PokemonRedBlue
{
    public class Factory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Pokemon Red/Blue Auto Splitter"; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Control; }
        }

        public string Description
        {
            get { return "Automatically splits for Pokemon Red/Blue on VBA 1.7.2, BGB 1.4.3 and Gambatte r550."; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new Component();
        }

        public string UpdateName
        {
            get { return ComponentName; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
#else
            get { return "http://livesplit.org/update/Components/update.LiveSplit.OcarinaOfTime.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return Version.Parse("1.0.0"); }
        }
    }
}
