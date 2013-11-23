using EntityEngineV4.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public class Image : Control
    {
        public ImageRender ImageRender;

        public Image(Page parent, string name, Point tabPosition, Texture2D texture)
            : base(parent, name, tabPosition)
        {
            ImageRender = new ImageRender(this, "ImageRender", texture);
            ImageRender.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
            Selectable = false;
        }
    }
}