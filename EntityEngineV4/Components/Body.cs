using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Body : Component
    {
        public float Angle;
        public Vector2 Position;
        public Vector2 Bounds;

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Bounds.X, (int)Bounds.Y);
            }
            set
            {
                Position = new Vector2(value.X, value.Y);
                Bounds = new Vector2(value.Width, value.Height);
            }
        }

        public float Top { get { return Position.Y; } }

        public float Left { get { return Position.X; } }

        public float Right { get { return Position.X + Bounds.X; } }

        public float Bottom { get { return Position.Y + Bounds.Y; } }

        public Body(Entity e, string name)
            : base(e, name)
        {
        }

        public Body(Entity e, string name, Vector2 position)
            : base(e, name)
        {
            Position = position;
        }

        public Body Clone()
        {
            var b = new Body(Parent, Name);
            b.Position = Position;
            b.Angle = Angle;
            return b;
        }
    }
}