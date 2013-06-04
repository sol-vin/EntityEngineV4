using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision
{
    public class Collision : Component
    {
        //Delegates and events
        public delegate void EventHandler(Collision c);
        public event EventHandler CollideEvent;

        //Dependencies
        private CollisionHandler _collisionHandler;
        public Body CollisionBody;

        public Rectangle BoundingRect 
        { 
            get { return CollisionBody.BoundingRect; } 
            set { CollisionBody.BoundingRect = value; } 
        }

        public Vector2 Position
        {
            get { return CollisionBody.Position; }
            set { CollisionBody.Position = value; }
        }

        public Vector2 Bounds
        {
            get { return CollisionBody.Bounds; }
            set { CollisionBody.Bounds = value; }
        }

        /// <summary>
        /// The group mask is the bit mask used to determine which groups the component is a part of.
        /// The CollisionHandler will pair all components with the same mask.
        /// </summary>
        /// <value>
        /// The group mask.
        /// </value>
        public int GroupMask { get; protected set; }

        /// <summary>
        /// The pair mask is the bit mask used to determine which groups the component will pair with.
        /// The CollisionHandler will only pair components whose group mask matches the pair mask.
        /// </summary>
        /// <value>
        /// The pair mask.
        /// </value>
        public int PairMask { get; protected set; }

        public Collision(Entity parent, string name, Body collisionBody) : base(parent, name)
        {
            CollisionBody = collisionBody;
            _collisionHandler = parent.StateRef.GetService<CollisionHandler>();
        }

        public void OnCollision(Collision c)
        {
            if (CollideEvent != null)
                CollideEvent(c);
        }

        /// <summary> 
        /// Toggles a specified bit to on in the GroupMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void AddGroupMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            GroupMask = GroupMask | mask;
        }

        /// <summary>
        /// Toggles a specified bit to off in the GroupMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void RemoveGroupMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            GroupMask = GroupMask | mask;
        }

        /// <summary>
        /// Toggles a specified bit to on in the PairMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void AddPairMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            PairMask = PairMask | mask;
        }

        /// <summary>
        /// Toggles a specified bit to off in the PairMask
        /// </summary>
        /// <param name="maskDepth">Which bit should be toggled</param>
        public void RemovePairMask(int maskDepth)
        {
            int mask = 1 << maskDepth;
            PairMask = PairMask | mask;
        }

        public void AddToHandler()
        {
            _collisionHandler.AddCollision(this);
        }
    }
}
