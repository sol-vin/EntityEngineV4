using System;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering.Primitives
{
    public static class ShapeTypes
    {
        public abstract class Primitive : Render
        {
            public Body Body;

            public float Thickness = 1;

            protected Primitive(IComponent parent, string name)
                : base(parent, name)
            {
                Body = new Body(this, "Primitive.Body");
            }

            protected Primitive(IComponent parent, string name, Vector2 pos)
                : base(parent, name)
            {
                Body = new Body(this, "Primitive.Body", pos);
            }

            protected Primitive(IComponent parent, string name, Body body)
                : base(parent, name)
            {
                Body = body;
            }

            public static void DrawLine(SpriteBatch sb, Vector2 p1, Vector2 p2, float thickness, float layer, Color color)
            {
                float angle = (float)System.Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
                float length = Vector2.Distance(p1, p2);

                sb.Draw(Assets.Pixel, p1, null, color,
                        angle, Vector2.Zero, new Vector2(length, thickness),
                        SpriteEffects.None, layer);
            }
        }

        public class Point : Primitive
        {
            public float X { get { return Body.X; } set { Body.X = value; } }

            public float Y { get { return Body.Y; } set { Body.Y = value; } }

            public float Angle { get { return Body.Angle; } set { Body.Angle = value; } }

            public override Microsoft.Xna.Framework.Rectangle DrawRect
            {
                get { return new Microsoft.Xna.Framework.Rectangle((int)(X - Thickness / 2), (int)(Y - Thickness / 2), (int)(X + Thickness), (int)(Y + Thickness)); }
            }

            public static implicit operator Vector2(Point p)
            {
                return new Vector2(p.X, p.Y);
            }

            public Point(Entity parent, string name, int x = 0, int y = 0)
                : base(parent, name)
            {
                X = x;
                Y = y;
            }

            public Point(IComponent parent, string name, Body body)
                : base(parent, name, body)
            {
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                sb.Draw(Assets.Pixel, DrawRect, null, Color * Alpha, Angle, new Vector2(DrawRect.Width / 2f, DrawRect.Height / 2f), Flip, Layer);
            }
        }

        public class Line : Primitive
        {
            public Vector2 Point1, Point2;

            public Line(IComponent parent, string name, Vector2 point1, Vector2 point2)
                : base(parent, name)
            {
                Point1 = point1;
                Point2 = point2;
            }

            public override void Draw(SpriteBatch sb)
            {
                if (DrawRect.Top < EntityGame.Camera.ScreenSpace.Height ||
                    DrawRect.Bottom > EntityGame.Camera.ScreenSpace.X ||
                    DrawRect.Right > EntityGame.Camera.ScreenSpace.Y ||
                    DrawRect.Left < EntityGame.Camera.ScreenSpace.Width)
                {
                    float angle = (float)Math.Atan2(Point2.Y - Point1.Y, Point2.X - Point1.X);
                    float length = Vector2.Distance(Point1, Point2);

                    sb.Draw(Assets.Pixel, Point1, null, Color,
                            angle, Vector2.Zero, new Vector2(length, Thickness),
                            SpriteEffects.None, Layer);
                }
            }
        }

        public class Triangle : Primitive
        {
            public Vector2 Point1, Point2, Point3;

            public Triangle(Entity parent, string name, Vector2 point1, Vector2 point2, Vector2 point3)
                : base(parent, name)
            {
                Point1 = point1;
                Point2 = point2;
                Point3 = point3;
            }

            public Triangle(Entity parent, string name, Vector2 point1, Vector2 point2, Vector2 point3, Color color)
                : base(parent, name)
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
            public float X { get { return Body.X; } set { Body.X = value; } }

            public float Y { get { return Body.Y; } set { Body.Y = value; } }

            public float Width { get { return Body.Width; } set { Body.Width = value; } }

            public float Height { get { return Body.Height; } set { Body.Height = value; } }

            public float Angle { get { return Body.Angle; } set { Body.Angle = value; } }

            public override Vector2 Bounds { get { return new Vector2((Width + Thickness) * Scale.X, (Height + Thickness) * Scale.Y); } }

            public override Microsoft.Xna.Framework.Rectangle DrawRect
            {
                get
                {
                    return new Microsoft.Xna.Framework.Rectangle((int)(X - Thickness / 2), (int)(Y - Thickness / 2),
                                                                 (int)((X + Width + Thickness) * Scale.X),
                                                                 (int)((Y + Height + Thickness) * Scale.Y));
                }
            }

            public bool Fill;

            public Rectangle(IComponent parent, string name, float x, float y, float width, float height, bool fill = false)
                : base(parent, name)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Fill = fill;

                Origin = new Vector2(.5f, .5f);
            }

            public Rectangle(IComponent parent, string name, Body body, bool fill)
                : base(parent, name, body)
            {
                Fill = fill;
                Origin = new Vector2(.5f, .5f);
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                if (!Fill)
                {
                    float minx = X + (Thickness / 2) + Origin.X;
                    float miny = Y + (Thickness / 2) + Origin.Y;
                    //TODO: Fix thickness issue
                    //Draw our top line
                    sb.Draw(Assets.Pixel,
                            new Vector2(minx, miny), null, Color * Alpha, Angle, new Vector2(Origin.X, Origin.Y * Bounds.Y) / Scale, new Vector2(Bounds.X, Thickness * Scale.Y), Flip, Layer);

                    //Left line
                    sb.Draw(Assets.Pixel,
                            new Vector2(minx, miny), null, Color * Alpha, Angle, new Vector2(Origin.X * Bounds.X, Origin.Y) / Scale, new Vector2(Thickness * Scale.X, Bounds.Y), Flip, Layer);

                    //Essentially these are the same as the top and bottom just rotated 180 degrees
                    //I have to do it this way instead of setting the origin to a negative value because XNA
                    //seems to ignore origins when they are negative
                    //Right Line
                    sb.Draw(Assets.Pixel,
                            new Vector2(minx + 1, miny), null, Color * Alpha, Angle + MathHelper.Pi, new Vector2(Origin.X * Bounds.X, Origin.Y) / Scale, new Vector2(Thickness * Scale.X, Bounds.Y), Flip, Layer);

                    //Bottom Line
                    sb.Draw(Assets.Pixel,
                            new Vector2(minx, miny + 1), null, Color * Alpha, Angle + MathHelper.Pi, new Vector2(Origin.X, Origin.Y * Bounds.Y) / Scale, new Vector2(Bounds.X, Thickness * Scale.Y), Flip, Layer);
                }
                else
                {
                    sb.Draw(Assets.Pixel, new Vector2(X + Origin.X * Width, Y + Origin.Y * Height), null, Color * Alpha, Angle, Origin, Bounds - new Vector2(Thickness), Flip, Layer);
                }
            }
        }
    }
}