using EntityEngineV4.Engine;

namespace EntityEngineV4.Input
{
    public class MouseInput : Input
    {
        private MouseButton _button;
        public MouseButton Button
        {
            get { return _button; }

            set
            {
                _button = value;
            }
        }
        public MouseInput(Entity entity, string name, MouseButton button) : base(entity, name)
        {
            _button = button;
        }

        public override bool Pressed()
        {
            return MouseHandler.IsMouseButtonPressed(Button);

        }

        public override bool Released()
        {
            return MouseHandler.IsMouseButtonReleased(Button);

        }

        public override bool Down()
        {
            return MouseHandler.IsMouseButtonDown(Button);

        }

        public override bool Up()
        {
            return MouseHandler.IsMouseButtonUp(Button);
        }
    }
}
