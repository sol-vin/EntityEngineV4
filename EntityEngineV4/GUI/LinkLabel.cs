using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class LinkLabel : Label
    {
        public Color UnselectedColor = Color.Black;
        public Color SelectedColor = Color.Red;
        public Collision Collision;
        public AABB BoundingBox;

        public LinkLabel(Page parent, string name, Point tabPosition)
            : base(parent, name, tabPosition)
        {
            Selectable = true;
            BoundingBox = new AABB(this, "AABB");
            BoundingBox.LinkDependency(AABB.DEPENDENCY_BODY, Body);

            Collision = new Collision(this, "Collision");
            Collision.Pair.AddMask(Cursor.MouseCollisionGroup);
            Collision.CollideEvent += OnMouseCollide;
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, BoundingBox);
            BoundingBox.LinkDependency(AABB.DEPENDENCY_COLLISION, Collision);

            BoundingBox.Debug = true;
        }

        public override void OnFocusLost(Control c)
        {
            base.OnFocusLost(c);
            Render.Color = UnselectedColor;
        }

        public override void OnFocusGain()
        {
            base.OnFocusGain();
            Render.Color = SelectedColor;
        }

        private void OnMouseCollide(Manifold m)
        {
            if (!HasFocus)
                (Parent as Page).FocusOn(this);

            if (MouseService.Cursor.Pressed()) Press();
            else if (MouseService.Cursor.Down()) Down();
            if (MouseService.Cursor.Released()) Release();
        }
    }
}