using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public class TextRender : Render
    {
        public string Text;
        public SpriteFont Font;

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position;
                position = _body.Position;
                return new Rectangle((int)position.X, (int)position.Y, (int)(Bounds.X), (int)(Bounds.Y));
            }
        }

        public override Vector2 Bounds
        {
            get { return new Vector2(Font.MeasureString(Text).X * Scale.X, Font.MeasureString(Text).Y * Scale.Y); }
        }

        //Dependencies
        private Body _body;

        public TextRender(Entity entity, string name, Body body)
            : base(entity, name)
        {
            _body = body;
        }

        public TextRender(Entity entity, string name, SpriteFont font, string text, Body body)
            : base(entity, name)
        {
            Text = text;
            Font = font;
            _body = body;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (EntityGame.Camera.ScreenSpace.Intersects(DrawRect))
                sb.DrawString(Font, Text, _body.Position + Origin, Color * Alpha, _body.Angle, Origin, Scale, Flip, Layer);
        }

        public void LoadFont(string location)
        {
            Font = EntityGame.Game.Content.Load<SpriteFont>(location);
        }
    }
}