using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Label : Control
    {
        public Color Color = Color.Black;

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

        public Label(IComponent parent, string name)
            : base(parent, name)
        {
            Render = new TextRender(this, "Render", Assets.Font, name);
            Render.Link(TextRender.DEPENDENCY_BODY, Body);
            Render.Text = name;
            Render.Layer = 1f;

            Body.Bounds = Render.Font.MeasureString(Text);
            Render.Color = Color.Black;
            Selectable = false;
        }
    }
}