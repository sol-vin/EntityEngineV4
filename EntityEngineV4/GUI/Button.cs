using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Button : Control
    {
        protected ShapeTypes.Rectangle _bodyImage;
        public Collision Collision;
        public AABB BoundingBox;

        public RGBColor RGBColor
        {
            get { return _bodyImage.Color.ToRGBColor(); }
            set { _bodyImage.Color = value; }
        }

        public Button(Page parent, string name, Point tabPosition, Vector2 position, Vector2 bounds, RGBColor color) 
            : base(parent, name, tabPosition)
        {
            Body.Position = position;
            Body.Bounds = bounds;

            //Make our rectangles
            _bodyImage = new ShapeTypes.Rectangle(this, "BodyImage", true);
            _bodyImage.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
            _bodyImage.Color = color;

            BoundingBox = new AABB(this, "AABB");
            BoundingBox.LinkDependency(AABB.DEPENDENCY_BODY, Body);

            Collision = new Collision(this, "Collision");
            Collision.Pair.AddMask(Cursor.MouseCollisionGroup);
            Collision.CollideEvent += OnMouseCollide;
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, BoundingBox);
            BoundingBox.LinkDependency(AABB.DEPENDENCY_COLLISION, Collision);
        }

        /// <summary>
        /// Changes the entity back to it's default configuration
        /// </summary>
        public void MakeDefault()
        {
            _bodyImage.Color = Color.White.ToRGBColor();
            
            FocusLost += c => RGBColor = Color.White.ToRGBColor();
            FocusGain += c => RGBColor = Color.Red.ToRGBColor();
            OnDown += c => RGBColor = Color.Green.ToRGBColor();
        }

        private void OnMouseCollide(Manifold m)
        {
            if(!HasFocus)
                (Parent as Page).FocusOn(this);

            if (MouseService.Cursor.Pressed()) Press();
            else if (MouseService.Cursor.Down()) Down();
            if (MouseService.Cursor.Released()) Release();
        }
    }
}
