using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public class Circle : Shape
    {
        public float Radius;
        public override Vector2 Position
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Position; }
        }

        public float X { get { return GetLink<Body>(DEPENDENCY_BODY).X + Radius; } }
        public float Y { get { return GetLink<Body>(DEPENDENCY_BODY).Y + Radius; } }

        public float Diameter { get { return Radius*2; } }

        public float Top { get { return GetLink<Body>(DEPENDENCY_BODY).Y + Radius; } }
        /// <summary>
        /// Add one extra radius to compensate for the origin "point"
        /// </summary>
        public float Bottom { get { return GetLink<Body>(DEPENDENCY_BODY).Y + Radius*3; } }
        public float Left { get { return GetLink<Body>(DEPENDENCY_BODY).X + Radius; } }
        public float Right { get { return GetLink<Body>(DEPENDENCY_BODY).X + Radius*3; } }
        

        public Circle(IComponent parent, string name, float radius = 0f) : base(parent, name)
        {
            Radius = radius;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
    }
}
