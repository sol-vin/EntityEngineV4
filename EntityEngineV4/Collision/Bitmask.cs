using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngineV4.Collision
{
    public class Bitmask
    {
        private int _mask = 0;

        public int Mask
        {
            get { return _mask; }
        }

        public Bitmask()
        {
        }

        public Bitmask(int mask)
        {
            _mask = mask;
        }

        public void RemoveMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            _mask = _mask | mask;
        }

        /// <summary>
        /// Toggles a specified bit to on in the PairMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void AddMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            _mask = _mask | mask;
        }

        public bool HasMatchingBit(Bitmask other)
        {
            return (other.Mask & Mask) > 0;
        }

        public bool CheckBit(int depth)
        {
            int mask = 1 << depth;
            return (mask & Mask) > 0;
        }
    }
}
