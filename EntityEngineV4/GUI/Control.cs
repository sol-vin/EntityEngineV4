using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;

namespace EntityEngineV4.GUI
{
    public abstract class Control : Entity
    {
        public Body Body;
        public TextRender TextRender;

        public bool HasFocus;
        public bool TabStop;

        public Control(EntityState stateref, string name) : base(stateref, stateref, name)
        {
        }
    }
}
