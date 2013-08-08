using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles
{
    public class Tile
    {
        public short Index;
        public const short EMPTY = -1;
        public Color Color = Color.White;
        public float Layer;
        public SpriteEffects Flip;
        public bool Solid;

        public Tile(short index)
        {
            Index = index;
            Flip = SpriteEffects.None;
        }
    }
}