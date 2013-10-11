using System;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.PowerTools
{
    public static class DrawingTools
    {
        public static void DrawLine(SpriteBatch sb, Vector2 p1, Vector2 p2, float thickness, float layer, Color color)
        {
            float angle = (float)System.Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);

            sb.Draw(Assets.Pixel, p1, null, color,
                    angle, Vector2.Zero, new Vector2(length, thickness),
                    SpriteEffects.None, layer);
        }

        public abstract class Primitive
        {
            public Color Color = Color.White;
            public float Layer;
            public bool Visible = true;
            public float Thickness = 1f;

            protected Primitive()
            {
            }

            public virtual void Draw(SpriteBatch sb)
            {
            }

            public virtual void Destroy()
            {
                Color = new Color();
                Visible = false;
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

            public Rectangle(Microsoft.Xna.Framework.Rectangle rect)
            {
                X = rect.X;
                Y = rect.Y;
                Width = rect.Width;
                Height = rect.Height;
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                if (!Fill)
                {
                    for (int x = (int)(X - Thickness / 2); x < X + Thickness / 2; x++)
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
                        DrawLine(sb, new Vector2(X, Y + y), new Vector2(X + Width, Y + y), Thickness, Layer, Color);
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
        public static class Physics
        {
            public static float DotProduct(Vector2 a, Vector2 b)
            {
                return a.X*b.X + a.Y*b.Y;
            }

            public static Vector2 GetNormal(Vector2 a, Vector2 b)
            {
                Vector2 ret = b - a;
                ret.Normalize();
                return ret;
            }

            public static float CrossProduct(Vector2 a, Vector2 b)
            {
                return a.X*b.Y - a.Y*b.X;
            }

            public static Vector2 CrossProduct(Vector2 a, float scalar)
            {
                return new Vector2(scalar*a.Y, -scalar*a.X);
            }

            public static Vector2 CrossProduct(float scalar, Vector2 a)
            {
                return new Vector2(-scalar*a.Y, scalar*a.X);
            }

            public static Vector2 RotatePoint(Vector2 origin, float angle, Vector2 point)
            {
                float s = (float)Math.Sin(angle);
                float c = (float)Math.Cos(angle);

                // translate point back to origin:
                point.X -= origin.X;
                point.Y -= origin.Y;
    
                // rotate point
                float xnew = point.X * c - point.Y * s;
                float ynew = point.X * s + point.Y * c;

                // translate point back:
                point.X = xnew + origin.X;
                point.Y = ynew + origin.Y;
                return point;
            }
        }

        public static float Lerp(float x, float y, float time)
        {
            return x + (y - x)*time;
        }

        private static Random _random = new Random();
        public static float NextGaussian(float average, float variation)
        {
            return average + 2.0f * ((float)_random.NextDouble() - 0.5f) * variation;
        }

}
}