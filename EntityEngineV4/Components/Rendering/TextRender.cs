using System.Security.Cryptography.X509Certificates;
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
                position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                return new Rectangle((int)position.X, (int)position.Y, (int)(Bounds.X), (int)(Bounds.Y));
            }
        }

        public override Vector2 Bounds
        {
            get { return new Vector2(Font.MeasureString(Text).X * Scale.X, Font.MeasureString(Text).Y * Scale.Y); }
        }

        public TextRender(Entity entity, string name)
            : base(entity, name)
        {
           
        }

        public TextRender(Entity entity, string name, SpriteFont font, string text)
            : base(entity, name)
        {
            Text = text;
            Font = font;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (EntityGame.Camera.ScreenSpace.Intersects(DrawRect))
                sb.DrawString(Font, Text, GetDependency<Body>(DEPENDENCY_BODY).Position + Origin, Color * Alpha, GetDependency<Body>(DEPENDENCY_BODY).Angle, Origin, Scale, Flip, Layer);
        }

        public void LoadFont(string location)
        {
            Font = EntityGame.Game.Content.Load<SpriteFont>(location);
        }

        //dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}