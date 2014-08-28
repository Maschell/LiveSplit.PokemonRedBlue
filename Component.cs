#define GAME_TIME

//using LiveSplit.ASL;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveSplit.UI.Components
{
    class Component : LogicComponent
    {
        //public ComponentSettings Settings { get; set; }

        public override string ComponentName
        {
            get { return "Pokemon Game Time"; }
        }


        public Component()
        {
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
        }

        public override System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("x");
        }

        public override System.Windows.Forms.Control GetSettingsControl(UI.LayoutMode mode)
        {
            return null;
        }
      
        
public override void Dispose()
{
 	throw new NotImplementedException();
}

public override void RenameComparison(string oldName, string newName)
{
 	throw new NotImplementedException();
}

public override void SetSettings(System.Xml.XmlNode settings)
{
 	throw new NotImplementedException();
}
}

    }
}
