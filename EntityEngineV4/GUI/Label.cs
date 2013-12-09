using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Label : Control
    {
        public Color Color { get { return Render.Color; } set { Render.Color = value; } }

        public string Text
        {
            get { return Render.Text; }
            set
            {
                Render.Text = value;

                Body.Bounds = Render.Font.MeasureString(Render.Text);
            }
        }

        //Components
        public TextRender Render;

        public Label(Page parent, string name, Point tabPosition)
            : base(parent, name, tabPosition)
        {
            Render = new TextRender(this, "Render", Assets.Font, name);
            Render.LinkDependency(TextRender.DEPENDENCY_BODY, Body);
            Render.Text = name;
            Render.Layer = 1f;

            Body.Bounds = Render.Font.MeasureString(Text);
            Render.Color = Color.Black;
            Selectable = false;
        }
    }
}