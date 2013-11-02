using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class VectorComponent : Component
    {
        public Vector2 Vector;
        public float X {get { return Vector.X; } set { Vector.X = value; }}
        public float Y { get { return Vector.Y; } set { Vector.Y = value; } }

        public static implicit operator Vector2(VectorComponent v)
        {
            return new Vector2(v.X, v.Y);
        }

        public VectorComponent(Node parent, string name) : base(parent, name)
        {
            Vector = new Vector2();
        }

        public VectorComponent(Node parent, string name ,Vector2 vector) : base(parent, name)
        {
            Vector = vector;
        }
    }
}
