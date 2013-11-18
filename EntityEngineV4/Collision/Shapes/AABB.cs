using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public class AABB : Shape
    {
        public override Vector2 Position
        {
            get { return GetDependency<Body>(DEPENDENCY_BODY).Position; }
        }

        public float X { get { return GetDependency<Body>(DEPENDENCY_BODY).X; } }
        public float Y { get { return GetDependency<Body>(DEPENDENCY_BODY).Y; } }

        public float Width {get { return GetDependency<Body>(DEPENDENCY_BODY).Width; }}
        public float Height { get { return GetDependency<Body>(DEPENDENCY_BODY).Height; } }

        public float Top { get { return GetDependency<Body>(DEPENDENCY_BODY).Top; } }
        public float Bottom { get { return GetDependency<Body>(DEPENDENCY_BODY).Bottom; } }        
        public float Left { get { return GetDependency<Body>(DEPENDENCY_BODY).Left; } }
        public float Right { get { return GetDependency<Body>(DEPENDENCY_BODY).Right; } }

        public AABB(Node parent, string name) : base(parent, name)
        {
        }

        //Dependencies
        public new const int DEPENDENCY_COLLISION = 0;
        public new const int DEPENDENCY_BODY = 1;
    }
}