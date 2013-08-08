using EntityEngineV4.Components.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public class Image : Control
    {
        public ImageRender ImageRender;

        public Image(ControlHandler parent, string name, Texture2D texture)
            : base(parent, name)
        {
            ImageRender = new ImageRender(this, "ImageRender", texture, Body);
            Selectable = false;
        }
    }
}