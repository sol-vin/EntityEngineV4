using EntityEngineV4.Data;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles
{
    public class Tile
    {
        public delegate void EventHandler(Tile t);
        public short Index;
        public const short EMPTY = -1;
        public RGBColor Color;
        public SpriteEffects Flip;
        public bool Solid;

        public Tile(short index)
        {
            Index = index;
            Flip = SpriteEffects.None;
            Solid = false;
        }

        public Tile Clone()
        {
            Tile t = new Tile(Index);
            t.Flip = Flip;
            t.Solid = Solid;
            t.Color = Color;
            return t;
        }
    }
}