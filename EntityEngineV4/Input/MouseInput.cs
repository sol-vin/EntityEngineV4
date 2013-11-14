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
        public MouseInput(Node node, string name, MouseButton button) : base(node, name)
        {
            _button = button;
        }

        public override bool Pressed()
        {
            return MouseService.IsMouseButtonPressed(Button);

        }

        public override bool Released()
        {
            return MouseService.IsMouseButtonReleased(Button);

        }

        public override bool Down()
        {
            return MouseService.IsMouseButtonDown(Button);

        }

        public override bool Up()
        {
            return MouseService.IsMouseButtonUp(Button);
        }
    }
}
