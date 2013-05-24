using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Data
{
    public struct Assets
    {
        public static Texture2D Pixel;

        public static void LoadConent(Game game)
        {
            Pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] pixeldata = new Color[1];
            pixeldata[0] = Color.White;
            Pixel.SetData(pixeldata);
        }
    }
}