using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public class Image : Control
    {
        public ImageRender ImageRender;

        public Image(EntityState stateref, string name, Texture2D texture) : base(stateref, name)
        {
            ImageRender = new ImageRender(this, "ImageRender", texture, Body);
            Selectable = false;
        }
    }
}
