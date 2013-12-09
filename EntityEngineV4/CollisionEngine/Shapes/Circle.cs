using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.CollisionEngine.Shapes
{
    public class Circle : Shape
    {
        public float Radius;

        public override Vector2 Position
        {
            get { return GetDependency<Body>(DEPENDENCY_BODY).Position + Offset; }
        }

        public float X { get { return GetDependency<Body>(DEPENDENCY_BODY).X + Offset.X; } }

        public float Y { get { return GetDependency<Body>(DEPENDENCY_BODY).Y + Offset.Y; } }

        public float Diameter { get { return Radius * 2; } }

        public float Top { get { return GetDependency<Body>(DEPENDENCY_BODY).Y; } }

        public float Bottom { get { return GetDependency<Body>(DEPENDENCY_BODY).Y + Radius * 2; } }

        public float Left { get { return GetDependency<Body>(DEPENDENCY_BODY).X; } }

        public float Right { get { return GetDependency<Body>(DEPENDENCY_BODY).X + Radius * 2; } }

        public Color DebugColorWhenNotColliding = new Color(255, 0, 0, 30);
        public Color DebugColorWhenColliding = new Color(0, 255, 0, 30);

        public Circle(Node parent, string name, float radius = 0f)
            : base(parent, name)
        {
            Radius = radius;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        private float _pixeltoscale = 1f;

        public override void Draw(SpriteBatch sb)
        {
            if (Debug)
            {
                if (GetDependency<Collision>(DEPENDENCY_COLLISION).IsColliding)
                {
                    _pixeltoscale = Diameter / Assets.Circle.Width;
                    sb.Draw(Assets.Circle,
                        Position - new Vector2(Radius),
                        null,
                        DebugColorWhenColliding,
                        0f,
                        Vector2.Zero,
                        _pixeltoscale,
                        SpriteEffects.None,
                        1f);
                }
                else
                {
                    _pixeltoscale = Diameter / Assets.Circle.Width;
                    sb.Draw(Assets.Circle,
                        Position - new Vector2(Radius),
                        null,
                        DebugColorWhenNotColliding,
                        0f,
                        Vector2.Zero,
                        _pixeltoscale,
                        SpriteEffects.None,
                        1f);
                }
            }
            base.Draw(sb);
        }

        //Dependencies
        public new const int DEPENDENCY_COLLISION = 0;

        public new const int DEPENDENCY_BODY = 1;
    }
}