using System;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    [System.Runtime.InteropServices.GuidAttribute("99977B74-33C1-4F01-B152-46D0EF0C4280")]
    public static class ColorMath
    {
        public static Color HSVtoRGB(float hue, float saturation, float value, float alpha, out int outhi)
        {
            if (hue > 1 || saturation > 1 || value > 1) throw new Exception("values cannot be more than 1!");
            if (hue < 0 || saturation < 0 || value < 0) throw new Exception("values cannot be less than 0!");

            Color output = new Color();
            outhi = 0;
            if (Math.Abs(saturation)  < 0.001)
            {
                output.R = (byte)(value * byte.MaxValue);
                output.G = (byte)(value * byte.MaxValue);
                output.B = (byte)(value * byte.MaxValue);
            }
            else
            {
                var hi = hue*6f;
                outhi = (int) hi;
                var i = (hi - (int) hi)/180;
                var var1 = value * (1f - saturation);
                var var2 = value * (1f - saturation * i);
                var var3 = value * (1f - saturation * (1f - i));

                switch ((int)hi)
                {
                    case (0):
                        output = new Color(value * 255f, var3 * 255f, var1 * 255f, alpha);
                        break;
                    case (1):
                        output = new Color(var2 * 255f, value * 255f, var1 * 255f, alpha);
                        break;
                    case (2):
                        output = new Color(var1 * 255f, value * 255f, var3 * 255f, alpha);
                        break;
                    case (3):
                        output = new Color(var1 * 255f, var2 * 255f, value * 255f, alpha);
                        break;
                    case (4):
                        output = new Color(var3 * 255f, var1 * 255f, value * 255f, alpha);
                        break;
                    case (5):
                        output = new Color(value * 255f, var1 * 255f, var2 * 255f, alpha);
                        break;
                    default:
                        throw new Exception("RGB color unknown!");
                }

            }
            return output;
        }

        public static Color TestHSVtoRGB(float hue, float saturation, float value, float alpha)
        {
            var output = new Color();

            hue *= 360;

            float chroma = value*saturation;
            float hdash = hue/60f;
            float x = chroma * (1f - Math.Abs((hdash % 2) - 1f));

            if (hdash < 1f)
            {
                output.R = (byte) (chroma*byte.MaxValue);
                output.G = (byte) (x*byte.MaxValue);
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
                output.R += (byte)(min*byte.MaxValue);
                output.G += (byte)(min * byte.MaxValue);
                output.B += (byte)(min * byte.MaxValue);
            }

            output.A = (byte) (alpha*byte.MaxValue);

            return output;
        }
    }
}