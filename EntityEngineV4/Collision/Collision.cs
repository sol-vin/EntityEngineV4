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

        //Properties

        public Rectangle BoundingRect
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).BoundingRect; }
            set { GetLink<Body>(DEPENDENCY_BODY).BoundingRect = value; }
        }

        public Vector2 Position
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Position; }
            set { GetLink<Body>(DEPENDENCY_BODY).Position = value; }
        }

        public Vector2 Bounds
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Bounds; }
            set { GetLink<Body>(DEPENDENCY_BODY).Bounds = value; }
        }

        public Vector2 Velocity
        {
            get { return GetLink<Physics>(DEPENDENCY_PHYSICS).Velocity; }
            set { GetLink<Physics>(DEPENDENCY_PHYSICS).Velocity = value; }
        }

        public Vector2 PositionDelta
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Delta; }
        }

        public Vector2 LastPosition
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).LastPosition; }
        }

        public Vector2 Delta
        {
            get { return GetLink<Body>(DEPENDENCY_BODY).Delta; }
        }

        public Collision(IComponent parent, string name, Shape shape)
            : base(parent, name)
        {
            _collisionHandler = GetService<CollisionHandler>();

            Shape = shape;
            Shape.Collision = this;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            ResolutionGroupMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            try
            {
                GetLink<Physics>(DEPENDENCY_PHYSICS);
            }
            catch
            {
                try
                {
                    Physics physics  = new Physics(this, "CollisionPhysics");
                    physics.Link(Physics.DEPENDENCY_BODY, GetLink<Body>(DEPENDENCY_BODY));
                }
                catch
                {
                    throw new Exception("Body and Physics do not exist in the dependency list for " + Name);
                }
            }
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
            _collidedWith.Clear();
        }

        public void OnCollision(Collision c)
        {
            _collidedWith.Add(c);
            if (CollideEvent != null)
                CollideEvent(c);
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public const int DEPENDENCY_PHYSICS = 1;

        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
            AddLinkType(DEPENDENCY_PHYSICS, typeof(Physics));
        }
    }
}