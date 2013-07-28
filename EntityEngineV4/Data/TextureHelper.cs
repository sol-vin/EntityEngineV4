using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Data
{
    public static class TextureHelper
    {
        public static Color[,] GetData(this Texture2D texture)
        {
            Color[] colordata = new Color[texture.Width * texture.Height];
            texture.GetData(colordata);

            Color[,] output = new Color[texture.Width, texture.Height];
            for (int i = 0; i < colordata.Length; i++)
            {
                if (i < texture.Width)
                {
                    output[i, 0] = colordata[i];
                    continue;
                }
                else if (i >= texture.Width)
                {
                    int yvalue = (int)Math.Floor(i/(double)texture.Width);
                    output[i - (yvalue*texture.Width), yvalue] = colordata[i];
                }
            }

            return (Color[,]) output.Clone();
        }
    }
}
