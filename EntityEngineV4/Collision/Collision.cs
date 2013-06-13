using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Collision
{
    public class Collision : Component
    {
        //Delegates and events
        public delegate void EventHandler(Collision c);
        public event EventHandler CollideEvent;

        /// <summary>
        /// The group mask is the bit mask used to determine which groups the component is a part of.
        /// The CollisionHandler will pair all components with the same mask.
        /// </summary>
        /// <value>
        /// The group mask.
        /// </value>
        public Bitmask GroupMask { get; protected set; }

        /// <summary>
        /// The pair mask is the bit mask used to determine which groups the component will pair with.
        /// The CollisionHandler will only pair components whose group mask matches the pair mask.
        /// </summary>
        /// <value>
        /// The pair mask.
        /// </value>
        public Bitmask PairMask { get; protected set; }

        /// <summary>
        /// The resolution mask is the bit mask which will determine which groups will physically collide with each other
        /// </summary>
        public Bitmask ResolutionGroupMask { get; protected set; }

        /// <summary>
        /// The resolution mask is the bit mask which will determine which pairs will physically collide with each other
        /// </summary>
        public Bitmask ResolutionPairMask { get; protected set; }

        //Collision Related Values

        //// <summary>
        /// Backing field for Mass.
        /// </summary>
        private float _mass = 1f;
        /// <summary>
        /// The mass of the object.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        public float Mass
        {
            get { return _mass; }
            set
            {
                if (value < 0) throw new Exception("Mass cannot be less than zero!");
                _mass = value;

                if (Math.Abs(value - 0) < .00001f)
                    InvertedMass = 0;
                else
                    InvertedMass = 1 / _mass;
            }
        }

        /// <summary>
        /// Gets one divided by mass (1/mass).
        /// </summary>
        /// <value>
        /// The inverted mass.
        /// </value>
        public float InvertedMass { get; private set; }

        /// <summary>
        /// Bounciness of this object
        /// </summary>
        public float Restitution = 0f;

        public Shape Shape;
        public bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return; //Opt out if the value isn't actually changing

                _enabled = value;
                _collisionHandler.GeneratePairs();
            }
        }

        //Dependencies
        private CollisionHandler _collisionHandler;
        private Body _collisionBody;
        private Physics _collisionPhysics;


        //Properties

        public Rectangle BoundingRect
        {
            get { return _collisionBody.BoundingRect; }
            set { _collisionBody.BoundingRect = value; }
        }

        public Vector2 Position
        {
            get { return _collisionBody.Position; }
            set { _collisionBody.Position = value; }
        }

        public Vector2 Bounds
        {
            get { return _collisionBody.Bounds; }
            set { _collisionBody.Bounds = value; }
        }

        public Vector2 Velocity
        {
            get { return _collisionPhysics.Velocity; }
            set { _collisionPhysics.Velocity = value; }
        }

        public Vector2 LastVelocity
        {
            get { return _collisionPhysics.LastVelocity; }
        }

        public Vector2 LastPosition
        {
            get { return Position - LastVelocity; }
        }

        public Collision(Entity parent, string name, Shape shape, Body collisionBody) : base(parent, name)
        {
            _collisionBody = collisionBody;
            _collisionHandler = parent.StateRef.GetService<CollisionHandler>();

            _collisionPhysics = new Physics(Parent, name + ".Physics", _collisionBody);

            Shape = shape;
            Shape.Collision = this;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.GeneratePairs();

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.GeneratePairs();

            ResolutionGroupMask = new Bitmask();
            ResolutionPairMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public Collision(Entity parent, string name, Shape shape, Body collisionBody, Physics collisionPhysics)
            : base(parent, name)
        {
            _collisionBody = collisionBody;
            _collisionHandler = parent.StateRef.GetService<CollisionHandler>();

            _collisionPhysics = collisionPhysics;

            Shape = shape;
            Shape.Collision = this;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.GeneratePairs();

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.GeneratePairs();

            ResolutionGroupMask = new Bitmask();
            ResolutionPairMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public void OnCollision(Collision c)
        {
            if (CollideEvent != null)
                CollideEvent(c);
        }
    }
}
