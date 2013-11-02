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
                    (int)(position.X + Origin.X * Scale.X),
                    (int)(position.Y + Origin.Y * Scale.Y),
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

        public override Rectangle SourceRect
        {
            get { return new Rectangle(0, 0, Texture.Width, Texture.Height); }
        }


        public ImageRender(Entity e, string name)
            : base(e, name)
        {
        }

        public ImageRender(Entity e, string name, Texture2D texture)
            : base(e, name)
        {
            Texture = texture;
        }



        public override void Draw(SpriteBatch sb)
        {
            if (DrawRect.Top < EntityGame.Camera.ScreenSpace.Height ||
                DrawRect.Bottom > EntityGame.Camera.ScreenSpace.X ||
                DrawRect.Right > EntityGame.Camera.ScreenSpace.Y ||
                DrawRect.Left < EntityGame.Camera.ScreenSpace.Width)
                sb.Draw(Texture, DrawRect, null, Color * Alpha, GetDependency<Body>(DEPENDENCY_BODY).Angle, Origin, Flip, Layer);
        }

        public void LoadTexture(string location)
        {
            Texture = EntityGame.Game.Content.Load<Texture2D>(location);
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}