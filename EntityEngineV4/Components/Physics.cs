using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Physics : Component
    {
        public float AngularVelocity;
        public float AngularDrag = 1f;

        public float AngularForce { get; private set; }

        public float AngularAcceleration;

        /// <summary>
        /// The velcocity if the object measured in px/frame
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;

        public float Drag = 1f;
        public Vector2 Acceleration = Vector2.Zero;

        public Vector2 Force { get; private set; }

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

                if (Math.Abs(value) < float.Epsilon)
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
        public float Restitution = 1f;

        public Physics(Node parent, string name)
            : base(parent, name)
        {
            Force = Vector2.Zero;
            AngularForce = 0f;
        }

        public override void Update(GameTime gt)
        {
            Velocity += Acceleration;
            Velocity += Force;
            Force = Vector2.Zero;
            Velocity *= Drag;

            AngularVelocity += AngularAcceleration;
            AngularVelocity += AngularForce;
            AngularVelocity *= AngularDrag;
            AngularForce = 0f;

            GetDependency<Body>(DEPENDENCY_BODY).Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            GetDependency<Body>(DEPENDENCY_BODY).Angle += AngularVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public void Thrust(float power)
        {
            Thrust(power, GetDependency<Body>(DEPENDENCY_BODY).Angle);
        }

        public void Thrust(float power, float angle)
        {
            AddForce((float)Math.Sin(angle) * power, (float)Math.Cos(angle) * -power);
        }

        public void FaceVelocity()
        {
            GetDependency<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(Velocity.X, -Velocity.Y);
        }

        public void FaceVelocity(Vector2 velocity)
        {
            GetDependency<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(velocity.X, velocity.Y);
        }

        public void AddForce(Vector2 force)
        {
            Force += force;
        }

        public void AddForce(float x, float y)
        {
            Force = new Vector2(Force.X + x, Force.Y + y);
        }

        public Physics Clone()
        {
            Physics p = new Physics(Parent, Name);
            p.AngularVelocity = AngularVelocity;
            p.AngularDrag = AngularDrag;
            p.Drag = Drag;
            p.Velocity = Velocity;
            p.Acceleration = Acceleration;
            p.LinkDependency(DEPENDENCY_BODY, GetDependency(DEPENDENCY_BODY));
            return p;
        }

        public void AddAngularForce(float force)
        {
            AngularForce += force;
        }

        public override void Reuse(Node parent, string name)
        {
            base.Reuse(parent, name);
            Velocity = Vector2.Zero;
            AngularVelocity = 0;
            Drag = 1;
            AngularDrag = 1;
            AngularForce = 0;
            Force = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Mass = 1;
            Restitution = 0;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;

        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }

        //Static methods
        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 GetNormal(Vector2 a, Vector2 b)
        {
            Vector2 ret = b - a;
            ret.Normalize();
            return ret;
        }

        public static float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static Vector2 CrossProduct(Vector2 a, float scalar)
        {
            return new Vector2(scalar * a.Y, -scalar * a.X);
        }

        public static Vector2 CrossProduct(float scalar, Vector2 a)
        {
            return new Vector2(-scalar * a.Y, scalar * a.X);
        }

        public static Vector2 RotatePoint(Vector2 origin, float angle, Vector2 point)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);

            // translate point back to origin:
            point.X -= origin.X;
            point.Y -= origin.Y;

            // rotate point
            float xnew = point.X * c - point.Y * s;
            float ynew = point.X * s + point.Y * c;

            // translate point back:
            point.X = xnew + origin.X;
            point.Y = ynew + origin.Y;
            return point;
        }

        public static float GetAngle(Vector2 vector)
        {
            return GetAngle(Vector2.Zero, vector);
        }

        public static float GetAngle(Vector2 vector1, Vector2 vector2)
        {
            return (float)Math.Atan2(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }
    }
}