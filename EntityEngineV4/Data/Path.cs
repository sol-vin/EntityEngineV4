using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Data
{
    public class Path
    {
        private List<PathPoint> _points = new List<PathPoint>();

        public Color DebugPointColor = Color.Red;
        public Color DebugLineColor = Color.Green;

        public PathPoint First
        {
            get { return _points[0]; }
        }

        public PathPoint Last
        {
            get { return _points[_points.Count - 1]; }
        }

        public int Count { get { return _points.Count; } }

        private int _lastposition;

        public Path()
        {
        }

        /// <summary>
        /// Adds a PathPoint to the back of the List
        /// </summary>
        /// <param name="p"></param>
        public void AddPoint(PathPoint p)
        {
            p.Rank = _lastposition++;
            _points.Add(p);
            _points = _points.OrderBy(point => point.Rank).ToList();
        }

        public void RemovePoint(int position)
        {
            foreach (var pathPoint in _points)
            {
            }
        }

        public void Insert(PathPoint p, int position)
        {
            if (position > _lastposition) _lastposition = position;
            p.Rank = _lastposition++;
            _points.Add(p);
            _points = _points.OrderBy(point => point.Rank).ToList();
        }

        public void DrawDebug(SpriteBatch sb)
        {
            if (_points.Count < 2 && _points.Count > 0)
            {
                //Only draw the one Point
                var singlepoint = new DrawingTools.Point((int)First.X, (int)First.Y);
                singlepoint.Thickness = 3;
                singlepoint.Color = DebugPointColor;
                singlepoint.Layer = 10;
            }
            else if (_points.Count < 0) return;

            //get our first line, then automate the rest.
            var point1 = new DrawingTools.Point((int) First.X, (int) First.Y);
            point1.Thickness = 3;
            point1.Color = DebugPointColor;
            point1.Layer = 1;
            point1.Draw(sb);

            var line = new DrawingTools.Line(new Vector2(First.X, First.Y), new Vector2(_points[1].X, _points[1].Y));
            line.Color = DebugLineColor;
            line.Layer = 1f;
            line.Draw(sb);

            var point2 = new DrawingTools.Point((int) _points[1].X, (int) _points[1].Y);
            point2.Thickness = 3;
            point2.Color = DebugPointColor;
            point2.Layer = 1;
            point2.Draw(sb);

            //Now we start getting the rest of the path
            for (int i = 2; i < Count; i++)
            {
                point1 = point2;

                point2 = new DrawingTools.Point((int) _points[i].X, (int) _points[i].Y);
                point2.Thickness = 3;
                point2.Color = DebugPointColor;
                point2.Layer = 1;
                point2.Draw(sb);

                line = new DrawingTools.Line(new Vector2(point1.X, point1.Y), new Vector2(point2.X, point2.Y), DebugLineColor);
                line.Layer = 1;
                line.Draw(sb);
            }
        }
    }

    public struct PathPoint
    {
        public float X, Y;
        public int Rank;

        public Vector2 Position { get { return new Vector2(X, Y); } set { X = value.X;
            Y = value.Y;
        } }

        public PathPoint(int x, int y)
        {
            X = x;
            Y = y;
            Rank = 0;
        }
    }
}