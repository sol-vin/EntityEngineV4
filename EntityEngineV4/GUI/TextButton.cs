using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    //TODO: Fix buttons
    public class TextButton : Button
    {
        private Body _textBody;
        public TextRender TextRender;
        private ShapeTypes.Rectangle _bodyImage;
        private ShapeTypes.Rectangle _bodyOutline;

        public Vector2 Spacing = new Vector2(5,1);

        public string Text
        {
            get { return TextRender.Text; }
            set
            {
                TextRender.Text = value;
                _textBody.Bounds = TextRender.Bounds;
                Body.Bounds = TextRender.Bounds + Spacing;

                _textBody.Position = new Vector2(Body.Position.X + Body.Bounds.X / 2 - _textBody.Bounds.X / 2,
                    Body.Position.Y + Body.Bounds.Y / 2 - _textBody.Bounds.Y / 2);
            }
        }

        public RGBColor RGBColor
        {
            get { return _bodyImage.Color.ToRGBColor(); }
            set { _bodyImage.Color = value; }
        }
        
        public TextButton(Page parent, string name, Point tabPosition, Vector2 position, RGBColor color) :base(parent, name, tabPosition, position, new Vector2(0,0), color)
        {
            Body.Position = position;

            _textBody = new Body(this, "TextBody");
            TextRender = new TextRender(this, "Render", Assets.Font, "");
            TextRender.LinkDependency(TextRender.DEPENDENCY_BODY, _textBody);
            TextRender.Layer = .9f;

            //Start setting up Render's body
            _textBody.Bounds = TextRender.Bounds;
            Body.Bounds = TextRender.Bounds + Spacing;

            _textBody.Position = new Vector2(Body.Position.X + Body.Bounds.X / 2 - _textBody.Bounds.X / 2, 
                Body.Position.Y + Body.Bounds.Y / 2 - _textBody.Bounds.Y / 2);

            //Make our rectangles
            _bodyImage = new ShapeTypes.Rectangle(this, "BodyImage", true);
            _bodyImage.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
            _bodyImage.Color = color;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Body.Bounds = TextRender.Bounds + Spacing;
            _textBody.Bounds = TextRender.Bounds;
            _textBody.Position = new Vector2(Body.Position.X + Body.Bounds.X / 2 - _textBody.Bounds.X / 2,
             Body.Position.Y + Body.Bounds.Y / 2 - _textBody.Bounds.Y / 2);
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
            TextRender.Color = Color.Black.ToRGBColor();
            FocusLost += c => RGBColor = Color.White.ToRGBColor();
            FocusGain += c => RGBColor = Color.Red.ToRGBColor();
            OnDown += c => RGBColor = Color.Green.ToRGBColor();
        }
    }
}
