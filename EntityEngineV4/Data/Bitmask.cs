using System;

namespace EntityEngineV4.Data
{
    public class Bitmask
    {
        public delegate void EventHandler(Bitmask bm);

        public event EventHandler BitmaskChanged;

        public uint Mask { get; private set; }

        public Bitmask()
        {
            Mask = 0;
        }

        public Bitmask(uint mask)
        {
            Mask = mask;
        }

        public void RemoveMask(int maskDepth)
        {
            if (maskDepth < 0) throw new Exception("Mask cannot be negative!");
            if (maskDepth > 31) throw new Exception("Mask cannot be larger than 31!");
            var mask = 1u << maskDepth;
            Mask = Mask | mask;

            if (BitmaskChanged != null) BitmaskChanged(this);
        }

        /// <summary>
        /// Toggles a specified bit to on in the PairMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void AddMask(int maskDepth)
        {
            if (maskDepth < 0) throw new Exception("Mask cannot be negative!");
            if (maskDepth > 31) throw new Exception("Mask cannot be larger than 31!");
            var mask = 1u << maskDepth;
            Mask = Mask | mask;

            if (BitmaskChanged != null)
                BitmaskChanged(this);
        }

        public bool HasMatchingBit(Bitmask other)
        {
            return (other.Mask & Mask) > 0;
        }

        public bool CheckBit(int depth)
        {
            if (depth < 0) throw new Exception("Mask cannot be negative!");
            if (depth > 31) throw new Exception("Mask cannot be larger than 31!");
            var mask = 1u << depth;
            return (mask & Mask) > 0;
        }
    }
}