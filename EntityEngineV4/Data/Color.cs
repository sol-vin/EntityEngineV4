using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    public struct HSVColor
    {
        private float _h;
        private float _s;
        private float _v;
        private float _a;

        public static implicit operator Color(HSVColor a)
        {
            return a.ToColor();
        }

        public HSVColor(float h, float s, float v, float a)
        {
            _h = h;
            _s = s;
            _v = v;
            _a = a;
        }

        public float A
        {
            get { return _a; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _a = value;
            }
        }

        public float V
        {
            get { return _v; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _v = value;
            }
        }

        public float S
        {
            get { return _s; }
            set
            {

                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _s = value;
            }
        }

        public float H
        {
            get { return _h; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _h = value;
            }
        }

        public Color ToColor()
        {
            return ColorMath.HSVtoRGB(this);
        }
    }

    public struct RGBColor
    {
        private float _r;
        private float _g;
        private float _b;
        private float _a;

        public static implicit operator Color(RGBColor a)
        {
            return a.ToColor();
        }

        public RGBColor(float r, float g, float b, float a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;

            R = r;
            G = g;
            B = b;
            A = a;
        }

        public float A
        {
            get { return _a; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _a = value;
            }
        }

        public float B
        {
            get { return _b; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _b = value;
            }
        }

        public float G
        {
            get { return _g; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _g = value;
            }
        }

        public float R
        {
            get { return _r; }
            set
            {
                if (value > 1f)
                {
                    value -= (float)Math.Floor(value);
                }
                else if (value < 0f)
                {
                    value += 1 - (float)Math.Ceiling(Math.Abs(value));
                }

                _r = value;
            }
        }

        public Color ToColor()
        {
            var color = new Color();
            color.R = (byte)(R * byte.MaxValue);
            color.G = (byte)(G * byte.MaxValue);
            color.B = (byte)(B * byte.MaxValue);
            color.A = (byte)(A * byte.MaxValue);
            return color;
        }
    }

    public static class ColorMath
    {
        public static HSVColor RGBtoHSV(RGBColor rgb)
        {
            float max, min;
            HSVColor hsv = new HSVColor();
            hsv.A = rgb.A;

            min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);

            hsv.V = max;
            if (hsv.V < .0001f)
            {
                hsv.H = hsv.S = 0;
                return hsv;
            }

            rgb.R /= hsv.V;
            rgb.G /= hsv.V;
            rgb.B /= hsv.V;
            min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);

            hsv.S = max - min;
            if (hsv.S < .0001f)
            {
                hsv.H = 0;
                return hsv;
            }

            rgb.R = (rgb.R - min) / (max - min);
            rgb.G = (rgb.G - min) / (max - min);
            rgb.B = (rgb.B - min) / (max - min);
            min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);

            if (Math.Abs(max - rgb.R) < .0001f)
            {
                hsv.H = 1f/6f * (rgb.G - rgb.B);
                if (hsv.H < 0)
                {
                    hsv.H += 1f;
                }
            }
            else if (Math.Abs(max - rgb.G) < .0001f)
            {
                hsv.H = 2f/6f + 1f/6f * (rgb.B - rgb.R);
            }
            else /* rgb_max == rgb.b */
            {
                hsv.H = 4f/6f + 1f/6f * (rgb.R - rgb.G);
            }

            return hsv;
        }

        public static RGBColor HSVtoRGB(float hue, float saturation, float value, float alpha)
        {
            var output = new Color();

            if (hue > 1)
            {
                hue = hue - (float)Math.Floor(hue); //Rounds down and gives us the remander
            }
            float chroma = value * saturation;
            float hdash = hue * 6f;
            float x = chroma * (1f - Math.Abs((hdash % 2) - 1f));

            if (hdash < 1f)
            {
                output.R = (byte)(chroma * Byte.MaxValue);
                output.G = (byte)(x * Byte.MaxValue);
            }
            else if (hdash < 2f)
            {
                output.G = (byte)(chroma * Byte.MaxValue);
                output.R = (byte)(x * Byte.MaxValue);
            }
            else if (hdash < 3f)
            {
                output.G = (byte)(chroma * Byte.MaxValue);
                output.B = (byte)(x * Byte.MaxValue);
            }
            else if (hdash < 4f)
            {
                output.B = (byte)(chroma * Byte.MaxValue);
                output.G = (byte)(x * Byte.MaxValue);
            }
            else if (hdash < 5f)
            {
                output.B = (byte)(chroma * Byte.MaxValue);
                output.R = (byte)(x * Byte.MaxValue);
            }
            else if (hdash < 6f)
            {
                output.R = (byte)(chroma * Byte.MaxValue);
                output.B = (byte)(x * Byte.MaxValue);
            }

            unchecked
            {
                float min = value - chroma;
                output.R += (byte)(min * Byte.MaxValue);
                output.G += (byte)(min * Byte.MaxValue);
                output.B += (byte)(min * Byte.MaxValue);
            }

            output.A = (byte)(alpha * Byte.MaxValue);

            return output.ToRGBColor();
        }

        public static Color HSVtoRGB(HSVColor color)
        {
            return HSVtoRGB(color.H, color.S, color.V, color.A);
        }

        //Extensions
        public static RGBColor ToRGBColor(this Color c)
        {
            RGBColor rgbcolor = new RGBColor();

            rgbcolor.R = ((float)c.R / Byte.MaxValue);
            rgbcolor.G = ((float)c.G / Byte.MaxValue);
            rgbcolor.B = ((float)c.B / Byte.MaxValue);
            rgbcolor.A = ((float)c.A / Byte.MaxValue);

            return rgbcolor;
        }

        public static HSVColor ToHSVColor(this Color c)
        {
            return RGBtoHSV(c.ToRGBColor());
        }
    }
}
