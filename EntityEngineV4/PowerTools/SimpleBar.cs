using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.PowerTools
{
    //TODO: Finish writing SimpleBar
    public class SimpleBar : Component
    {
        private Body _fgBarBody, _bgBarBody;
        private ShapeTypes.Rectangle _fgBar, _bgBar;

        //Properties
        public Vector2 Position { get { return _bgBarBody.Position; } 
            set 
            { 
                _fgBarBody.Position = value;
                _bgBarBody.Position = value;
            } 
        }

        public float Angle { get { return _bgBarBody.Angle; } 
            set 
            { 
                _bgBarBody.Angle = value;
                _fgBarBody.Angle = value;
            }
        }

        public float Layer { get { return _bgBar.Layer; } 
            set 
            {
                _bgBar.Layer = value;
                _fgBar.Layer = value + .001f;
            }
        }
        public Color FillColor { get { return _fgBar.Color; } set { _fgBar.Color = value; } }

        public Color BarColor
        {
            get { return _bgBar.Color; }
            set { _bgBar.Color = value; }
        }

        public float Value = 100;
        public float MaxValue = 100;

        public float Percent { get { return Value / MaxValue; } set { Value = MaxValue*value; } }

        public enum FillType
        {
            XFillLeft, XFillRight, YFillUp, YFillDown, XFillMiddleInwards, XFillMiddleOutwards, 
            YFillMiddleInwards, YFillMiddleOutwards
        }

        public FillType Fill;

        public SimpleBar(Node parent, string name, Body body) : base(parent, name)
        {
            _bgBarBody = body;
            _fgBarBody = new Body(this, "FGBarBody");

            _fgBarBody.X = body.X;
            _fgBarBody.Y = body.Y;
            _fgBarBody.Height = body.Height;
            _fgBarBody.Width = body.Width;

            _fgBar = new ShapeTypes.Rectangle(this, "FGBar", true);
            _fgBar.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, _fgBarBody);

            _bgBar = new ShapeTypes.Rectangle(this, "BGBar", true);
            _bgBar.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, _bgBarBody);

        }
    }
}
