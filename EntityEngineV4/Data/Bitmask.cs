using System;

namespace EntityEngineV4.Data
{
    public class Bitmask
    {
        public delegate void EventHandler(Bitmask bm);

        public event EventHandler BitmaskChanged;

        private uint _mask = 0;
        public uint Mask
        {
            get { return _mask; }
            set 
        { 
            _mask = value;
            if (BitmaskChanged != null) BitmaskChanged(this);
        }
        }

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

        public void CombineMask(uint mask)
        {
            Mask |= mask;
        }

        public void CombineMask(Bitmask mask)
        {
            Mask |= mask.Mask;
        }

        public bool HasMatchingBit(Bitmask other)
        {
            return (other.Mask & Mask) > 0;
        }

        public bool HasMatchingBit(uint mask)
        {
            return (mask & Mask) > 0;
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