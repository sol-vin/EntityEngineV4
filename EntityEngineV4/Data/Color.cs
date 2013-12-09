using System;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    public enum ColorOutOfBoundsAction
    {
        DoNothing, ThrowError, WrapAround, Clamp
    }

    public interface PolyColor
    {
        /// <summary>
        /// Action to take if a value is out of range for a color
        /// </summary>
        ColorOutOfBoundsAction Action { get; set; }

        /// <summary>
        /// Alpha Channel
        /// </summary>
        float Alpha { get; set; }

        float CheckValue(float value, float minvalue, float maxvalue);
    }

    public struct HSVColor : PolyColor
    {
        private float _h;

        /// <summary>
        /// Hue
        /// </summary>
        public float H
        {
            get { return _h; }
            set { _h = CheckValue(value); }
        }

        private float _s;

        /// <summary>
        /// Saturation
        /// </summary>
        public float S
        {
            get { return _s; }
            set { _s = CheckValue(value); }
        }

        private float _v;

        /// <summary>
        /// Value
        /// </summary>
        public float V
        {
            get { return _v; }
            set { _v = CheckValue(value); }
        }

        private float _alpha;

        /// <summary>
        /// Alpha Channel
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = CheckValue(value); }
        }

        /// <summary>
        /// Backing field for Action
        /// </summary>
        private ColorOutOfBoundsAction _action;

        /// <summary>
        /// Action to take if a value is out of range for a color
        /// </summary>
        public ColorOutOfBoundsAction Action
        {
            get { return _action; }
            set
            {
                //Set out value, then re-check every color value
                _action = value;

                H = _h;
                S = _s;
                V = _v;
                Alpha = _alpha;
            }
        }

        public static implicit operator Color(HSVColor a)
        {
            return a.ToColor();
        }

        public HSVColor(float h, float s, float v, float alpha, ColorOutOfBoundsAction action = ColorOutOfBoundsAction.DoNothing)
        {
            _h = h;
            _s = s;
            _v = v;
            _alpha = alpha;

            _action = action;
        }

        public float CheckValue(float value, float minvalue = 0, float maxvalue = 1)
        {
            switch (_action)
            {
                case (ColorOutOfBoundsAction.DoNothing):
                    return value;

                case (ColorOutOfBoundsAction.ThrowError):
                    if (value > maxvalue || value < minvalue)
                        throw new ArgumentOutOfRangeException("value was outside the range!");
                    return value;

                case (ColorOutOfBoundsAction.WrapAround):
                    if (value > maxvalue)
                    {
                        value -= (float)Math.Floor(value);
                    }
                    else if (value < minvalue)
                    {
                        value += maxvalue - (float)Math.Ceiling(Math.Abs(value));
                    }
                    return value;

                case (ColorOutOfBoundsAction.Clamp):
                    return MathHelper.Clamp(value, minvalue, maxvalue);
            }
            throw new Exception("Something went horribly wrong....");
        }

        public Color ToColor()
        {
            return ColorMath.HSVtoRGB(this);
        }

        public RGBColor ToRGBColor()
        {
            return ColorMath.HSVtoRGB(this);
        }
    }

    public struct RGBColor : PolyColor
    {
        private float _r;

        public float R
        {
            get { return _r; }
            set { _r = CheckValue(value); }
        }

        private float _g;

        public float G
        {
            get { return _g; }
            set { _g = CheckValue(value); }
        }

        private float _b;

        public float B
        {
            get { return _b; }
            set { _b = CheckValue(value); }
        }

        private float _alpha;

        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = CheckValue(value); }
        }

        /// <summary>
        /// Backing field for Action
        /// </summary>
        private ColorOutOfBoundsAction _action;

        /// <summary>
        /// Action to take if a value is out of range for a color
        /// </summary>
        public ColorOutOfBoundsAction Action
        {
            get { return _action; }
            set
            {
                //Set out value, then re-check every color value
                _action = value;

                R = _r;
                G = _g;
                B = _b;
                Alpha = _alpha;
            }
        }

        public static implicit operator Color(RGBColor a)
        {
            return a.ToColor();
        }

        public RGBColor(float r, float g, float b, float alpha, ColorOutOfBoundsAction action = ColorOutOfBoundsAction.DoNothing)
        {
            _r = r;
            _g = g;
            _b = b;
            _alpha = alpha;

            _action = action;
        }

        public float CheckValue(float value, float minvalue = 0, float maxvalue = 1)
        {
            switch (_action)
            {
                case (ColorOutOfBoundsAction.DoNothing):
                    return value;

                case (ColorOutOfBoundsAction.ThrowError):
                    if (value > maxvalue || value < minvalue)
                        throw new ArgumentOutOfRangeException("Value was outside the range!");
                    return value;

                case (ColorOutOfBoundsAction.WrapAround):
                    if (value > maxvalue)
                    {
                        value -= (float)Math.Floor(value);
                    }
                    else if (value < minvalue)
                    {
                        value += maxvalue - (float)Math.Ceiling(Math.Abs(value));
                    }
                    return value;

                case (ColorOutOfBoundsAction.Clamp):
                    return MathHelper.Clamp(value, minvalue, maxvalue);
            }
            throw new Exception("Something went horribly wrong....");
        }

        public Color ToColor()
        {
            var color = new Color();
            color.R = (byte)(R * byte.MaxValue);
            color.G = (byte)(G * byte.MaxValue);
            color.B = (byte)(B * byte.MaxValue);
            color.A = (byte)(Alpha * byte.MaxValue);
            return color;
        }
    }

    /// <summary>
    /// Get's a new XYZ Color
    /// </summary>
    public struct XYZColor : PolyColor
    {
        public float _x, _y, _z, _alpha;

        public const float XLIMIT = 95.0428f;
        public const float YLIMIT = 100f;
        public const float ZLIMIT = 108.883f;

        public float X { get { return _x; } set { _x = CheckValue(value, 0, XLIMIT); } }

        public float Y { get { return _y; } set { _y = CheckValue(value, 0, YLIMIT); } }

        public float Z { get { return _z; } set { _z = CheckValue(value, 0, ZLIMIT); } }

        public float Alpha { get { return _alpha; } set { _alpha = CheckValue(value, 0, 1f); } }

        public ColorOutOfBoundsAction _action;

        public ColorOutOfBoundsAction Action
        {
            get { return _action; }
            set
            {
                _action = value;

                X = _x;
                Y = _y;
                Z = _z;
                Alpha = _alpha;
            }
        }

        public XYZColor White { get { return new XYZColor(95.047f, 100f, 108.883f, 1f); } }

        public XYZColor(float x, float y, float z, float alpha, ColorOutOfBoundsAction action = ColorOutOfBoundsAction.DoNothing)
        {
            _x = x;
            _y = y;
            _z = z;
            _alpha = alpha;

            _action = action;
        }

        public float CheckValue(float value, float minvalue, float maxvalue)
        {
            switch (_action)
            {
                case (ColorOutOfBoundsAction.DoNothing):
                    return value;

                case (ColorOutOfBoundsAction.ThrowError):
                    if (value > maxvalue || value < minvalue)
                        throw new ArgumentOutOfRangeException("Value was outside the range!");
                    return value;

                case (ColorOutOfBoundsAction.WrapAround):
                    if (value > maxvalue)
                    {
                        value -= (float)Math.Floor(value);
                    }
                    else if (value < minvalue)
                    {
                        value += maxvalue - (float)Math.Ceiling(Math.Abs(value));
                    }
                    return value;

                case (ColorOutOfBoundsAction.Clamp):
                    return MathHelper.Clamp(value, minvalue, maxvalue);
            }
            throw new Exception("Something went horribly wrong....");
        }
    }

    public struct CieLabColor : PolyColor
    {
        public float _l, _a, _b, _alpha;

        public const float LLIMIT = 100;

        public float L
        {
            get { return _l; }
            set { _l = CheckValue(value, 0, LLIMIT); }
        }

        public float A
        {
            get { return _a; }
            set { _a = value; }
        }

        public float B
        {
            get { return _b; }
            set { _b = value; }
        }

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = CheckValue(value, 0, 1);
            }
        }

        private ColorOutOfBoundsAction _action;

        public ColorOutOfBoundsAction Action
        {
            get { return _action; }
            set
            {
                _action = value;

                //Recalculate values
                L = _l;
                A = _a;
                B = _b;
                Alpha = _alpha;
            }
        }

        public CieLabColor(float l, float a, float b, ColorOutOfBoundsAction action = ColorOutOfBoundsAction.DoNothing)
            : this()
        {
            _l = l;
            _a = a;
            _b = b;
            Action = action;
        }

        public float CheckValue(float value, float minvalue, float maxvalue)
        {
            switch (_action)
            {
                case (ColorOutOfBoundsAction.DoNothing):
                    return value;

                case (ColorOutOfBoundsAction.ThrowError):
                    if (value > maxvalue || value < minvalue)
                        throw new ArgumentOutOfRangeException("Value was outside the range!");
                    return value;

                case (ColorOutOfBoundsAction.WrapAround):
                    if (value > maxvalue)
                    {
                        value -= (float)Math.Floor(value);
                    }
                    else if (value < minvalue)
                    {
                        value += maxvalue - (float)Math.Ceiling(Math.Abs(value));
                    }
                    return value;

                case (ColorOutOfBoundsAction.Clamp):
                    return MathHelper.Clamp(value, minvalue, maxvalue);
            }
            throw new Exception("Something went horribly wrong....");
        }
    }

    public struct CieLCHColor
    {
        //TODO: Finish Cie-LCH Color

        public float L, C, H;
    }

    public static class ColorMath
    {
        public static HSVColor RGBtoHSV(RGBColor rgb)
        {
            float max, min;
            HSVColor hsv = new HSVColor();

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
                hsv.H = 1f / 6f * (rgb.G - rgb.B);
                if (hsv.H < 0)
                {
                    hsv.H += 1f;
                }
            }
            else if (Math.Abs(max - rgb.G) < .0001f)
            {
                hsv.H = 2f / 6f + 1f / 6f * (rgb.B - rgb.R);
            }
            else /* rgb_max == rgb.b */
            {
                hsv.H = 4f / 6f + 1f / 6f * (rgb.R - rgb.G);
            }

            hsv.Alpha = rgb.Alpha;

            return hsv;
        }

        public static RGBColor HSVtoRGB(HSVColor hsv)
        {
            float r, g, b;
            float h, s, v;

            r = g = b = 0;

            h = hsv.H;
            s = hsv.S;
            v = hsv.V;

            if (hsv.H > 1)
            {
                hsv.H = hsv.H - (float)Math.Floor(hsv.H); //Rounds down and gives us the remander
            }
            float chroma = hsv.V * hsv.S;
            float hdash = hsv.H * 6f;
            float x = chroma * (1f - Math.Abs((hdash % 2) - 1f));

            if (hdash < 1f)
            {
                r = chroma;
                g = x;
            }
            else if (hdash < 2f)
            {
                g = chroma;
                r = x;
            }
            else if (hdash < 3f)
            {
                g = chroma;
                b = x;
            }
            else if (hdash < 4f)
            {
                b = chroma;
                g = x;
            }
            else if (hdash < 5f)
            {
                b = chroma;
                r = x;
            }
            else if (hdash < 6f)
            {
                r = chroma;
                b = x;
            }
            float min = hsv.V - chroma;
            r += min;
            g += min;
            b += min;

            return new RGBColor(r, g, b, hsv.Alpha);
        }

        public static XYZColor RGBtoXYZ(RGBColor rgb)
        {
            XYZColor xyz = new XYZColor();

            float r, g, b;
            r = rgb.R;
            g = rgb.G;
            b = rgb.B;

            if (r > 0.04045f) r = (float)Math.Pow((r + 0.055) / 1.055, 2.4);
            else r = r / 12.92f;
            if (g > 0.04045) g = (float)Math.Pow((g + 0.055f) / 1.055f, 2.4f);
            else g = g / 12.92f;
            if (b > 0.04045f) b = (float)Math.Pow((b + 0.055f) / 1.055f, 2.4f);
            else b = b / 12.92f;

            r *= 100;
            g *= 100;
            b *= 100;

            xyz.X = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            xyz.Y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            xyz.Z = r * 0.0193f + g * 0.1192f + b * 0.9505f;

            xyz.Alpha = rgb.Alpha;
            return xyz;
        }

        public static RGBColor XYZtoRGB(XYZColor xyz)
        {
            float x, y, z;

            x = xyz.X / 100;        //X from 0 to  95.047      (Observer = 2°, Illuminant = D65)
            y = xyz.Y / 100;        //Y from 0 to 100.000
            z = xyz.Z / 100;     //Z from 0 to 108.883

            float r, g, b;

            r = x * 3.2406f + y * -1.5372f + z * -0.4986f;
            g = x * -0.9689f + y * 1.8758f + z * 0.0415f;
            b = x * 0.0557f + y * -0.2040f + z * 1.0570f;

            if (r > 0.0031308f) r = 1.055f * (float)Math.Pow(r, (1 / 2.4f)) - 0.055f;
            else r = 12.92f * r;
            if (g > 0.0031308f) g = 1.055f * (float)Math.Pow(g, (1 / 2.4f)) - 0.055f;
            else g = 12.92f * g;
            if (b > 0.0031308f) b = 1.055f * (float)Math.Pow(b, (1 / 2.4f)) - 0.055f;
            else b = 12.92f * b;

            //clamp values before making it a RGBColor

            r = MathHelper.Clamp(r, 0, 1);
            g = MathHelper.Clamp(g, 0, 1);
            b = MathHelper.Clamp(b, 0, 1);

            return new RGBColor(r, g, b, xyz.Alpha);
        }

        //TODO: Test XYZtoLAB
        public static CieLabColor XYZtoLAB(XYZColor xyz)
        {
            float var_x = xyz.X / XYZColor.XLIMIT;
            float var_y = xyz.Y / XYZColor.YLIMIT;
            float var_z = xyz.Z / XYZColor.ZLIMIT;

            if (var_x > 0.008856) var_x = (float)Math.Pow(var_x, (1 / 3f));
            else var_x = (7.787f * var_x) + (16f / 116f);

            if (var_y > 0.008856) var_y = (float)Math.Pow(var_y, (1 / 3f));
            else var_y = (7.787f * var_y) + (16f / 116f);

            if (var_z > 0.008856) var_z = (float)Math.Pow(var_z, (1 / 3f));
            else var_z = (7.787f * var_z) + (16f / 116f);

            float var_l = (116f * var_y) - 16f;
            float var_a = 500 * (var_x - var_y);
            float var_b = 200 * (var_y - var_z);

            return new CieLabColor(var_l, var_a, var_b) { Alpha = xyz.Alpha };
        }

        public static XYZColor LABtoXYZ(CieLabColor lab)
        {
            throw new NotImplementedException();
        }

        //Extensions
        public static RGBColor ToRGBColor(this Color c)
        {
            RGBColor rgbcolor = new RGBColor();

            rgbcolor.R = ((float)c.R / Byte.MaxValue);
            rgbcolor.G = ((float)c.G / Byte.MaxValue);
            rgbcolor.B = ((float)c.B / Byte.MaxValue);
            rgbcolor.Alpha = ((float)c.A / Byte.MaxValue);

            return rgbcolor;
        }

        public static HSVColor ToHSVColor(this Color c)
        {
            return RGBtoHSV(c.ToRGBColor());
        }
    }
}