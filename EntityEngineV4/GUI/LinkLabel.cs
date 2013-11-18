using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class LinkLabel : Label
    {
        public Color SelectedColor = Color.Red;

        public LinkLabel(Node parent, string name)
            : base(parent, name)
        {
            Selectable = true;
        }

        public override void OnFocusLost(Control c)
        {
            base.OnFocusLost(c);
            Render.Color = Color;
        }

        public override void OnFocusGain(Control c)
        {
            base.OnFocusGain(c);
            Render.Color = SelectedColor;
        }
    }
}