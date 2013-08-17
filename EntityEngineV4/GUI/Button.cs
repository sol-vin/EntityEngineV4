using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Button : Control
    {
        private ShapeTypes.Rectangle _bodyImage;

        public RGBColor NoFocusColor, FocusColor, DownColor;

        public Button(IComponent parent, string name, Vector2 position, Vector2 bounds, RGBColor color) : base(parent, name)
        {
            Body.Position = position;
            Body.Bounds = bounds;


            //Make our rectangles
            _bodyImage = new ShapeTypes.Rectangle(this, "BodyImage", Body, true);
            _bodyImage.Color = color;
            NoFocusColor = color;
            FocusColor = color;
            DownColor = color;
        }

        public override void OnFocusGain(Control c)
        {
            base.OnFocusGain(c);
            _bodyImage.Color = FocusColor;
        }

        public override void OnFocusLost(Control c)
        {
            base.OnFocusLost(c);
            _bodyImage.Color = NoFocusColor;
        }

        public override void Down()
        {
            base.Down();
            _bodyImage.Color = DownColor;
        }

    }
}
