using System;
using System.Collections.Generic;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.PowerTools
{
    public static class DrawingTools
    {

        public static void DrawLine(SpriteBatch sb, Vector2 p1, Vector2 p2, float thickness, float layer, Color color)
        {
            float angle = (float) System.Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);

            sb.Draw(Assets.Pixel, p1, null, color,
                    angle, Vector2.Zero, new Vector2(length, thickness),
                    SpriteEffects.None, layer);
        }

        public class PrimitiveHandler : Service
        {
            private List<Primitive> _shapes = new List<Primitive>();

            public PrimitiveHandler(EntityState stateRef)
                : base(stateRef, "PrimitiveHandler")
            {
            }

            public override void Update(GameTime gt)
            {
            }

            public override void Draw(SpriteBatch sb)
            {
                foreach (var primitive in _shapes)
                {
                    if (primitive.Visible)
                        primitive.Draw(sb);
                }
            }

            public void AddPrimitive(Primitive p)
            {
                _shapes.Add(p);
            }

            public void RemovePrimitive(Primitive p)
            {
                _shapes.Remove(p);
            }
        }

        public abstract class Primitive
        {
            public Color Color = Color.White;
            public float Layer;
            public bool Visible = true;
            public float Thickness = 1f;

            public PrimitiveHandler Parent { get; private set; }

            protected Primitive()
            {
            }

            public virtual void Draw(SpriteBatch sb)
            {
            }

            public void SetParent(PrimitiveHandler p)
            {
                Parent = p;
            }

            public virtual void Destroy()
            {
                Color = new Color();
                Visible = false;

                if (Parent != null)
                {
                    Parent.RemovePrimitive(this);
                }
            }
        }


        public class Point : Primitive
        {
            public int X, Y;
            public float Angle;

            public Microsoft.Xna.Framework.Rectangle DrawRect
            {
                get { return new Microsoft.Xna.Framework.Rectangle((int)(X - Thickness / 2), (int)(Y - Thickness / 2), (int)(Thickness), (int)(Thickness)); }
            }

            public static implicit operator Vector2(Point p)
            {
                return new Vector2(p.X, p.Y);
            }

            public Point(int x = 0, int y = 0)
            {
                X = x;
                Y = y;
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                sb.Draw(Assets.Pixel, DrawRect, null, Color, Angle, Vector2.Zero, SpriteEffects.None, Layer);
            }
        }

        public class Line : Primitive
        {
            public Vector2 Point1 = new Vector2();
            public Vector2 Point2 = new Vector2();



            public Line(Vector2 point1, Vector2 point2, Color color)
            {
                Point1 = point1;
                Point2 = point2;
                Color = color;
            }

            public Line(Vector2 point1, Vector2 point2)
            {
                Point1 = point1;
                Point2 = point2;
            }

            public override void Draw(SpriteBatch sb)
            {
                DrawLine(sb, Point1, Point2, Thickness, Layer, Color);
            }

            public override void Destroy()
            {
                base.Destroy();

                Point1 = new Vector2();
                Point2 = new Vector2();
            }
        }

        public class Triangle : Primitive
        {
            public Vector2 Point1, Point2, Point3;

            public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
            {
                Point1 = point1;
                Point2 = point2;
                Point3 = point3;
            }

            public Triangle(Vector2 point1, Vector2 point2, Vector2 point3, Color color)
            {
                Point1 = point1;
                Point2 = point2;
                Point3 = point3;
                Color = color;
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);

                //Draw each of the line
                DrawLine(sb, Point1, Point2, Thickness, Layer, Color);
                DrawLine(sb, Point2, Point3, Thickness, Layer, Color);
                DrawLine(sb, Point3, Point1, Thickness, Layer, Color);
            }
        }

        public class Rectangle : Primitive
        {
            public float X, Y, Width, Height;

            public bool Fill = false;

            public Rectangle(float x, float y, float width, float height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                if(!Fill)
                {
                    
                    for (int x = (int)(X - Thickness/2); x < X + Thickness/2; x++)
                    {
                        for (int y = (int)(Y - Thickness / 2); y < Y + Thickness / 2; y++)
                        {
                            //Draw our top line
                            DrawLine(sb, new Vector2(x, y), new Vector2(x + Width, y), 1, Layer, Color);

                            //Left line
                            DrawLine(sb, new Vector2(x, y), new Vector2(x, y + Height), 1, Layer, Color);

                            //Right Line
                            DrawLine(sb, new Vector2(x + Width, y), new Vector2(x + Width, y + Height), 1, Layer, Color);

                            //Bottom Line
                            DrawLine(sb, new Vector2(x - 1, y + Height), new Vector2(x + Width, y + Height), 1, Layer, Color);
                        }
                    }
                }
                else
                {
                    for (float y = 0; y < Height; y++)
                    {
                        DrawLine(sb, new Vector2(X,Y+y), new Vector2(X+Width, Y+y), Thickness, Layer, Color);
                    }
                }
            }
        }

        public class Circle : Primitive
        {
            public Vector2 Center;
            public float Radius;

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);

                throw new NotImplementedException();
            }
        }
    }

    public static class MathTools
    {
        public static class Color
        {
            public static Microsoft.Xna.Framework.Color HSVtoRGB(float hue, float saturation, float value, float alpha)
            {
                var output = new Microsoft.Xna.Framework.Color();

                if (hue > 1)
                {
                    hue = hue - (float) System.Math.Floor(hue); //Rounds down and gives us the remander
                }
                float chroma = value*saturation;
                float hdash = hue*6f;
                float x = chroma*(1f - Math.Abs((hdash%2) - 1f));

                if (hdash < 1f)
                {
                    output.R = (byte) (chroma*byte.MaxValue);
                    output.G = (byte) (x*byte.MaxValue);
                }
                else if (hdash < 2f)
                {
                    output.G = (byte) (chroma*byte.MaxValue);
                    output.R = (byte) (x*byte.MaxValue);
                }
                else if (hdash < 3f)
                {
                    output.G = (byte) (chroma*byte.MaxValue);
                    output.B = (byte) (x*byte.MaxValue);
                }
                else if (hdash < 4f)
                {
                    output.B = (byte) (chroma*byte.MaxValue);
                    output.G = (byte) (x*byte.MaxValue);
                }
                else if (hdash < 5f)
                {
                    output.B = (byte) (chroma*byte.MaxValue);
                    output.R = (byte) (x*byte.MaxValue);
                }
                else if (hdash < 6f)
                {
                    output.R = (byte) (chroma*byte.MaxValue);
                    output.B = (byte) (x*byte.MaxValue);
                }

                unchecked
                {
                    float min = value - chroma;
                    output.R += (byte) (min*byte.MaxValue);
                    output.G += (byte) (min*byte.MaxValue);
                    output.B += (byte) (min*byte.MaxValue);
                }

                output.A = (byte) (alpha*byte.MaxValue);

                return output;
            }
        }
        public static class Physics
        {
            public static float DotProduct(Vector2 a, Vector2 b)
            {
                return a.X * b.X + a.Y * b.Y;
            }
            public static Vector2 GetNormal(Vector2 a, Vector2 b)
            {
                Vector2 ret = b - a;
                ret.Normalize();
                return ret;
            }
        }
    }
}