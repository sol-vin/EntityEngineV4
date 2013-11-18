using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Button : Control
    {
        protected ShapeTypes.Rectangle _bodyImage;


        public RGBColor RGBColor
        {
            get { return _bodyImage.Color.ToRGBColor(); }
            set { _bodyImage.Color = value; }
        }

        public Button(Node parent, string name, Vector2 position, Vector2 bounds, RGBColor color) : base(parent, name)
        {
            Body.Position = position;
            Body.Bounds = bounds;


            //Make our rectangles
            _bodyImage = new ShapeTypes.Rectangle(this, "BodyImage", true);
            _bodyImage.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
            _bodyImage.Color = color;
        }

        public override void OnFocusGain(Control c)
        {
            base.OnFocusGain(c);
        }

        public override void OnFocusLost(Control c)
        {
            base.OnFocusLost(c);
        }

        public override void Down()
        {
            base.Down();
        }

        /// <summary>
        /// Changes the entity back to it's default configuration
        /// </summary>
        public void MakeDefault()
        {
            _bodyImage.Color = Color.White.ToRGBColor();
            
            FocusLost += c => RGBColor = Color.White.ToRGBColor();
            FocusGain += c => RGBColor = Color.Red.ToRGBColor();
            OnDown += c => RGBColor = Color.Green.ToRGBColor();
        }
    }
}
