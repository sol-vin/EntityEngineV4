using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{

    public class ControlHandler : Service
    {
        public bool UseMouse;

        public Page ActivePage { get { return _pages.FirstOrDefault(p => p.HasFocus); } }

        HashSet<Page> _pages = new HashSet<Page>();

        public ControlHandler(State stateref, bool useMouse = true)
            : base(stateref, "ControlHandler")
        {
            UseMouse = useMouse;

            //Check to see if the mouse service is present, if not, log and disable mouse
            if (!stateref.CheckService<MouseService>())
            {
                UseMouse = false;
                EntityGame.Log.Write("Mouse was disabled on this state, switching UseMouse to false. Next time, please specify UseMouse in constructor", this, Alert.Warning);
            }
        }

        public override void Update(GameTime gt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
        }

        public void NotifyFocusChange(Page activePage)
        {
            if (!_pages.Contains(activePage))
                _pages.Add(activePage);
            if (ActivePage != null)
                ActivePage.Hide();
        }
    }
}