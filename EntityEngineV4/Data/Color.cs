using System;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    public static class ColorMath
    {
        public static Color HSVtoRGB(float hue, float saturation, float value, float alpha)
        {
            var output = new Color();

            if (hue > 1)
            {
                hue = hue - (float)Math.Floor(hue); //Rounds down and gives us the remander
            }
            float chroma = value * saturation;
            float hdash = hue*6f;
            float x = chroma * (1f - Math.Abs((hdash % 2) - 1f));

            if (hdash < 1f)
            {
                output.R = (byte)(chroma * byte.MaxValue);
                output.G = (byte)(x * byte.MaxValue);
            }
            else if (hdash < 2f)
            {
                output.G = (byte)(chroma * byte.MaxValue);
                output.R = (byte)(x * byte.MaxValue);
            }
            else if (hdash < 3f)
            {
                output.G = (byte)(chroma * byte.MaxValue);
                output.B = (byte)(x * byte.MaxValue);
            }
            else if (hdash < 4f)
            {
                output.B = (byte)(chroma * byte.MaxValue);
                output.G = (byte)(x * byte.MaxValue);
            }
            else if (hdash < 5f)
            {
                output.B = (byte)(chroma * byte.MaxValue);
                output.R = (byte)(x * byte.MaxValue);
            }
            else if (hdash < 6f)
            {
                output.R = (byte)(chroma * byte.MaxValue);
                output.B = (byte)(x * byte.MaxValue);
            }

            unchecked
            {
                float min = value - chroma;
                output.R += (byte)(min * byte.MaxValue);
                output.G += (byte)(min * byte.MaxValue);
                output.B += (byte)(min * byte.MaxValue);
            }

            output.A = (byte)(alpha * byte.MaxValue);

            return output;
        }
    }
}