using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public class ImageRender : Render
    {
        public Texture2D Texture { get; private set; }

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                return new Rectangle(
                    (int)(position.X + GetDependency<Body>(DEPENDENCY_BODY).Origin.X * Scale.X),
                    (int)(position.Y + GetDependency<Body>(DEPENDENCY_BODY).Origin.Y * Scale.Y),
                    (int)(Bounds.X),
                    (int)(Bounds.Y));
            }
        }

        public override Vector2 Bounds
        {
            get
            {
                return new Vector2(Texture.Width * Scale.X, Texture.Height * Scale.Y);
            }
        }

        public ImageRender(Node e, string name)
            : base(e, name)
        {
        }

        public ImageRender(Node e, string name, Texture2D texture)
            : base(e, name)
        {
            Texture = texture;
        }



        public override void Draw(SpriteBatch sb)
        {
            if (DrawRect.Top < EntityGame.ActiveCamera.ScreenSpace.Height ||
                DrawRect.Bottom > EntityGame.ActiveCamera.ScreenSpace.X ||
                DrawRect.Right > EntityGame.ActiveCamera.ScreenSpace.Y ||
                DrawRect.Left < EntityGame.ActiveCamera.ScreenSpace.Width)
                sb.Draw(Texture, DrawRect, null, Color * Alpha, GetDependency<Body>(DEPENDENCY_BODY).Angle, GetDependency<Body>(DEPENDENCY_BODY).Origin, Flip, Layer);
        }

        public void LoadTexture(string location)
        {
            Texture = EntityGame.Game.Content.Load<Texture2D>(location);
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
            SourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
    }
}