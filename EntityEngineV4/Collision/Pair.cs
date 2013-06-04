using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngineV4.Collision
{
    public class Pair
    {
        protected bool Equals(Pair other)
        {
            return (Equals(A, other.A) && Equals(B, other.B)) || (Equals(A, other.B) && Equals(B, other.A));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Pair) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A != null ? A.GetHashCode() : 0)*(B != null ? B.GetHashCode() : 0);
            }
        }

        public readonly Collision A, B;

        public Pair(Collision a, Collision b)
        {
            A = a;
            B = b;
        }

        public static bool operator ==(Pair a, Pair b)
        {
            if (ReferenceEquals(a, b))
                return true;
            
            if ((object)a == null || (object)b == null)
                return false;

            return (a.A == b.A && a.B == b.B) || (a.A == b.B && a.B == b.A);
        }

        public static bool operator !=(Pair a, Pair b)
        {
            return !(a == b);
        }
    }
}
