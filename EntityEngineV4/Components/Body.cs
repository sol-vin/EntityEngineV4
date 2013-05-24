using System;
using Microsoft.Xna.Framework;
using EntityEngineV4.Engine;

namespace EntityEngineV4.Components
{
public class Body : Component
    {
        public float Angle;
        public Vector2 Position;

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
            var b = new Body(Parent as Entity, Name);
            b.Position = Position;
            b.Angle = Angle;
            return b;
        }
    }
}

