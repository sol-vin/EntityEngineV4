using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Engine
{
    public static class PhysicsMath
    {
        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(((int)(a.X - b.X) ^ 2 + (int)(a.Y - b.Y) ^ 2));
        }

        public static Vector2 GetNormal(Vector2 a, Vector2 b)
        {
            Vector2 ret = b - a;
            ret.Normalize();
            return ret;
        }
    }
}
