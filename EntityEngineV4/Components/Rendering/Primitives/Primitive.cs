using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering.Primitives
{
    public abstract class Primitive : Render
    {
        public float Thickness = 1;

        protected Primitive(Entity parent, string name)
            : base(parent, name)
        {
        }

        public static void DrawLine(SpriteBatch sb, Vector2 p1, Vector2 p2, float thickness, float layer, Color color)
        {
            float angle = (float) System.Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);

            sb.Draw(Assets.Pixel, p1, null, color,
                    angle, Vector2.Zero, new Vector2(length, thickness),
                    SpriteEffects.None, layer);
        }
    }

    public static class DrawTypes
    {

        public class Line : Primitive
        {
            public Vector2 Point1, Point2;

            public Line(Entity parent, string name, Vector2 point1, Vector2 point2)
                : base(parent, name)
            {
                Point1 = point1;
                Point2 = point2;
            }

            public override void Draw(SpriteBatch sb)
            {
                if (DrawRect.Top < EntityGame.CurrentCamera.ScreenSpace.Height ||
                    DrawRect.Bottom > EntityGame.CurrentCamera.ScreenSpace.X ||
                    DrawRect.Right > EntityGame.CurrentCamera.ScreenSpace.Y ||
                    DrawRect.Left < EntityGame.CurrentCamera.ScreenSpace.Width)
                {

                    float angle = (float) Math.Atan2(Point2.Y - Point1.Y, Point2.X - Point1.X);
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
            public float X, Y, Width, Height;
            public float Angle;

            public override Microsoft.Xna.Framework.Rectangle DrawRect
            {
                get
                {
                    return new Microsoft.Xna.Framework.Rectangle((int) (X - Thickness/2), (int) (Y - Thickness/2),
                                                                 (int) ((X + Width + Thickness/2) * Scale.X),
                                                                 (int) ((Y + Height + Thickness/2)* Scale.Y));
                }
            }

            public bool Fill;

            public Rectangle(Entity parent, string name, float x, float y, float width, float height, bool fill = false)
                : base(parent, name)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Fill = fill;

                Origin = new Vector2(Width/2, Height/2);
            }

            public override void Draw(SpriteBatch sb)
            {
                base.Draw(sb);
                if (!Fill)
                {
                    float minx = X - (Thickness/2); // +Origin.X * Scale.X;
                    float maxx = X + (Thickness/2); // +Origin.X * Scale.X;
                    float miny = Y - (Thickness/2); // +Origin.Y * Scale.Y;
                    float maxy = Y - (Thickness/2); // +Origin.Y * Scale.Y;
                    //TODO: Fix origin issue
                    //Draw our top line
                    sb.Draw(Assets.Pixel,
                            new Microsoft.Xna.Framework.Rectangle((int) minx, (int) miny, (int) (Width + Thickness),
                            (int)Thickness), null, Color, 0, Vector2.One, Flip, Layer);

                    //Left line
                    sb.Draw(Assets.Pixel,
                    new Microsoft.Xna.Framework.Rectangle((int)minx, (int)miny, (int)(Thickness),
                    (int)(Height + Thickness)), null, Color, 0, Vector2.Zero, Flip, Layer);

                    //Right Line
                    sb.Draw(Assets.Pixel,
                    new Microsoft.Xna.Framework.Rectangle((int)(minx+Width), (int)miny, (int)(Thickness),
                    (int)(Height + Thickness)), null, Color, 0, Vector2.Zero, Flip, Layer);

                    //Bottom Line
                    sb.Draw(Assets.Pixel,
                    new Microsoft.Xna.Framework.Rectangle((int)minx, (int)(miny+Height), (int)(Width + Thickness),
                    (int)Thickness), null, Color, 0, Vector2.Zero, Flip, Layer);
                }
                else
                {

                    sb.Draw(Assets.Pixel, DrawRect, null, Color, Angle, Origin, Flip, Layer);
                }
            }
        }
    }
}
