using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision
{
    public struct Manifold
    {
        private Pair _pair;
        public float PenetrationDepth;
        public Vector2 Normal;
        public bool AreColliding;

        public Collision A
        {
            get { return _pair.A; }
        }

        public Collision B
        {
            get { return _pair.B; }
        }

        public Manifold(Collision a, Collision b)
            : this()
        {
            _pair = new Pair(a, b);
        }
    }
}