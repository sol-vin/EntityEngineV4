using System;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    public static class PhysicsMath
    {
        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        public static Vector2 GetNormal(Vector2 a, Vector2 b)
        {
            Vector2 ret = b - a;
            ret.Normalize();
            return ret;
        }
    }
}