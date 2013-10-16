using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision.Shapes
{
    public class Circle : Shape
    {
        public float Radius;
        public override Vector2 Position { get; set; }

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)(Position.X - Radius), (int) (Position.Y-Radius),(int) (Radius*2), (int) (Radius*2));
            }
        }


    }
}
