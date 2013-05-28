using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public class ImageRender : Render
    {
        public Texture2D Texture { get; set; }

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position = Body.Position;
                return new Rectangle(
                    (int)((int)position.X + Origin.X * Scale.X),
                    (int)((int)position.Y + Origin.Y * Scale.Y),
                    (int)(Texture.Width * Scale.X),
                    (int)(Texture.Height * Scale.Y));
            }
        }

        public override Rectangle SourceRect
        {
            get { return new Rectangle(0, 0, Texture.Width, Texture.Height); }
        }

        //Dependencies
        protected Body Body;

        public ImageRender(Entity e, string name, Body body)
            : base(e, name)
        {
            Origin = Vector2.Zero;
            Body = body;
        }

        public ImageRender(Entity e, string name, Texture2D texture, Body body)
            : base(e, name)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Body = body;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, DrawRect, null, Color * Alpha, Body.Angle,
                    Origin, Flip, Layer);
        }

        public void LoadTexture(string location)
        {
            Texture = EntityGame.Game.Content.Load<Texture2D>(location);
        }
    }
}