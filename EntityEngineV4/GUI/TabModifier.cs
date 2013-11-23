using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class TabModifier : Control
    {
        private TabModification _modifier = TabModification.None;

        public TabModification Modifier
        {
            get
            {
                return _modifier;
            }

            set
            {
                if (CheckModificationSanity(value))
                    _modifier = value;
                else
                {
                    throw new Exception("Tab Modifier has invalid flags!");
                }
            }
        }
        protected TabModifier(Page parent, string name, Point tabPosition) : base(parent, name, tabPosition)
        {
        }

        private bool CheckModificationSanity(TabModification mod)
        {
            if ((mod & TabModification.Insanity) > 0) //check if it has any of the move mods
            {
                switch ((mod & TabModification.Insanity)) //Check if it has only one of the move mods
                {
                    case TabModification.MoveDown:
                    case TabModification.MoveUp:
                    case TabModification.MoveLeft:
                    case TabModification.MoveRight:
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
    }

    [Flags]
    public enum TabModification
    {
        None = 0,
        UpBlock = 1, LeftBlock = 2, RightBlock = 4, DownBlock = 8, //Blocks a Tab from coninuing, reversing it's direction.
        MoveUp = 16, MoveLeft = 32, MoveDown = 64, MoveRight = 128, //Sends the page cursor in the direction specified.

        FullBlock = UpBlock | LeftBlock | RightBlock | DownBlock,
        XBlock = LeftBlock | RightBlock,
        YBlock = UpBlock | DownBlock,

        Insanity = MoveUp | MoveDown | MoveRight | MoveLeft
    }
} 
