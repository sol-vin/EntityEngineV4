using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public class AABB : Shape
    {
        public override Vector2 Position
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Position; }
        }

        public float X { get { return GetLink<Body>(DEPENDENCY_BODY).X; } }
        public float Y { get { return GetLink<Body>(DEPENDENCY_BODY).Y; } }

        public float Width {get { return GetLink<Body>(DEPENDENCY_BODY).Width; }}
        public float Height { get { return GetLink<Body>(DEPENDENCY_BODY).Height; } }

        public float Top { get { return GetLink<Body>(DEPENDENCY_BODY).Top; } }
        public float Bottom { get { return GetLink<Body>(DEPENDENCY_BODY).Bottom; } }        
        public float Left { get { return GetLink<Body>(DEPENDENCY_BODY).Left; } }
        public float Right { get { return GetLink<Body>(DEPENDENCY_BODY).Right; } }

        public AABB(IComponent parent, string name) : base(parent, name)
        {
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
    }
}