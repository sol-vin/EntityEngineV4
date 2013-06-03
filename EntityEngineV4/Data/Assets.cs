using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Data
{
    public struct Assets
    {
        public static Texture2D Pixel;
        public static SpriteFont Font;

        public static void LoadConent(Game game)
        {
            Pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            var pixeldata = new Color[1];
            pixeldata[0] = Color.White;
            Pixel.SetData(pixeldata);

            Font = game.Content.Load<SpriteFont>(@"EntityEngine/font");
        }
    }
}