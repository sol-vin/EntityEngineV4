using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public class TileRender : ImageRender
    {
        public Vector2 TileSize { get; private set; }

        public int Index;

        public int Columns
        {
            get
            {
                return (int)(Texture.Width / TileSize.X);
            }
        }

        public int Rows { get { return (int)(Texture.Height / TileSize.Y); } }

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position = Body.Position;
                return new Rectangle(
                    (int)(position.X + Origin.X * Scale.X),
                    (int)(position.Y + Origin.Y * Scale.Y),
                    (int)(TileSize.X * Scale.X),
                    (int)(TileSize.Y * Scale.Y));
            }
        }

        public Rectangle SourceRectangle
        {
            get
            {
                var r = new Rectangle();
                for (var i = 0; i <= Index; i += Columns)
                {
                    var ypos = Index - i;

                    if (ypos >= Columns) continue;

                    var p = new Point { Y = (i / Columns) * (int)TileSize.Y, X = ypos * (int)TileSize.X };
                    r = new Rectangle(p.X, p.Y, (int)TileSize.X, (int)TileSize.Y);
                }
                return r;
            }
        }

        public TileRender(Entity e, string name, Body body)
            : base(e, name, body)
        {
        }

        public TileRender(Entity e, string name, Texture2D texture, Vector2 tilesize, Body body)
            : base(e, name, texture, body)
        {
            TileSize = tilesize;
            Origin = new Vector2(TileSize.X / 2.0f, TileSize.Y / 2.0f);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (EntityGame.Viewport.Intersects(DrawRect))
                sb.Draw(Texture, DrawRect, SourceRectangle, Color * Alpha, Body.Angle, Origin, Flip, Layer);
        }
    }
}