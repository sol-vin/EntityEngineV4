using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Entity : Node
    {
        public delegate void EventHandler(Entity e);

        public Entity(Node parent, string name) : base(parent, name)
        {
        }
    }
}