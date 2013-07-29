using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    public class Collision : Component
    {
        //Delegates and events
        public delegate void CollisionEventHandler(Collision c);

        public event CollisionEventHandler CollideEvent;

        public Bitmask CollisionDirection = new Bitmask();


        public Bitmask AllowCollisionDirection = new Bitmask(CollisionHandler.ALL);

        private List<Collision> _collidedWith = new List<Collision>(); 
        public List<Collision> CollidedWith
        {
            get { return _collidedWith.ToList(); }
        }
        public bool IsColliding { get { return CollidedWith.Count > 0; } }

        /// <summary>
        /// The group mask is the bit mask used to determine which groups the component is a part of.
        /// The CollisionHandler will pair all components with the same pair mask.
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

        public Color DebugColor = Color.Magenta;

        public Shape Shape;

        //TODO: Add this to the collision handler when pairing
        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return; //Opt out if the value isn't actually changing

                _enabled = value;
                _collisionHandler.ReconfigurePairs(this);
            }
        }

        public bool Immovable;

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

        public Vector2 PositionDelta
        {
            get { return _collisionBody.Delta; }
        }

        public Vector2 LastPosition
        {
            get { return _collisionBody.LastPosition; }
        }

        public Vector2 Delta
        {
            get { return _collisionBody.Delta; }
        }

        public Collision(IComponent parent, string name, Shape shape, Body collisionBody)
            : base(parent, name)
        {
            _collisionBody = collisionBody;
            _collisionHandler = EntityGame.CurrentState.GetService<CollisionHandler>();

            _collisionPhysics = new Physics(Parent, name + ".Physics", _collisionBody);

            Shape = shape;
            Shape.Collision = this;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            ResolutionGroupMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public Collision(IComponent parent, string name, Shape shape, Body collisionBody, Physics collisionPhysics)
            : base(parent, name)
        {
            _collisionBody = collisionBody;
            _collisionHandler = EntityGame.CurrentState.GetService<CollisionHandler>();

            _collisionPhysics = collisionPhysics;

            Shape = shape;
            Shape.Collision = this;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            ResolutionGroupMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);
            _collisionHandler.RemoveCollision(this);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (Debug)
            {
                //Draw our debug bounds.
                Rectangle drawwindow;
                //Draw top
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Y, BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw bottom
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Bottom, BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw left
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Y, 1, BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw right
                drawwindow = new Rectangle(BoundingRect.Right, BoundingRect.Y, 1, BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);
            }
            _collidedWith.Clear();
        }

        public void OnCollision(Collision c)
        {
            _collidedWith.Add(c);
            if (CollideEvent != null)
                CollideEvent(c);
        }
    }
}