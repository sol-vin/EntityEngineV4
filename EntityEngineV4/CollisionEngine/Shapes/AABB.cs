using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.CollisionEngine.Shapes
{
    public class AABB : Shape
    {
        public override Vector2 Position
        {
            get { return GetDependency<Body>(DEPENDENCY_BODY).Position + Offset; }
        }

        public float X { get { return GetDependency<Body>(DEPENDENCY_BODY).X + Offset.X; } }
        public float Y { get { return GetDependency<Body>(DEPENDENCY_BODY).Y + Offset.Y; } }

        public float Width { get { return GetDependency<Body>(DEPENDENCY_BODY).Width; } }
        public float Height { get { return GetDependency<Body>(DEPENDENCY_BODY).Height; } }

        public float Top { get { return GetDependency<Body>(DEPENDENCY_BODY).Top + Offset.Y; } }
        public float Bottom { get { return GetDependency<Body>(DEPENDENCY_BODY).Bottom + Offset.Y; } }
        public float Left { get { return GetDependency<Body>(DEPENDENCY_BODY).Left + Offset.X; } }
        public float Right { get { return GetDependency<Body>(DEPENDENCY_BODY).Right + Offset.X; } }

        public AABB(Node parent, string name)
            : base(parent, name)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (Debug)
            {
                var rect = new DrawingTools.Rectangle(GetDependency<Body>(DEPENDENCY_BODY).BoundingRect);
                rect.Fill = false;
                rect.Color = GetDependency<Collision>(DEPENDENCY_COLLISION).IsColliding ? Color.Green : Color.Red;
                rect.Layer = 1f;
                rect.Draw(sb);
            }
        }

        //Dependencies
        public new const int DEPENDENCY_COLLISION = 0;
        public new const int DEPENDENCY_BODY = 1;
    }
}