using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public abstract class Shape
    {
        public Collision Collision;

        public abstract Vector2 Position { get; set; }

        public virtual Rectangle BoundingBox { get; set; }

        public static implicit operator Collision(Shape c)
        {
            return c.Collision;
        }

        protected Shape()
        {
        }
    }
}