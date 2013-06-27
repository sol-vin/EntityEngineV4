using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public class AABB : Shape
    {
        public override Vector2 Position
        {
            get { return Collision.Position; }
            set { Collision.Position = value; }
        }

        public float Width
        {
            get { return Collision.Bounds.X; }
            set { Collision.Bounds = new Vector2(value, Collision.Bounds.Y); }
        }

        public float Height
        {
            get { return Collision.Bounds.Y; }
            set { Collision.Bounds = new Vector2(Collision.Bounds.X, value); }
        }

        public float Top { get { return Position.Y; } }

        public float Left { get { return Position.X; } }

        public float Right { get { return Position.X + Collision.Bounds.X; } }

        public float Bottom { get { return Position.Y + Collision.Bounds.Y; } }

        public override Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Collision.Bounds.X, (int)Collision.Bounds.Y); }
            set
            {
                Collision.Position = new Vector2(value.X, value.Y);
                Collision.Bounds = new Vector2(value.Width, value.Height);
            }
        }

        public AABB()
            : base()
        {
        }

        public static AABB CreateAABB(Rectangle r, Collision c)
        {
            AABB a = new AABB();
            a.Collision = c;
            return a;
        }
    }
}