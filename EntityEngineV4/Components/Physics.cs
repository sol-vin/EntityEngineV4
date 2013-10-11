using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Physics : Component
    {
        public float AngularVelocity;
        public float AngularVelocityDrag = 1f;

        /// <summary>
        /// The velcocity if the object measured in px/frame
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;

        public float Drag = 1f;
        public Vector2 Acceleration = Vector2.Zero;
        private Vector2 _force = Vector2.Zero;

        public Physics(IComponent e, string name)
            : base(e, name)
        {
        }

        public override void Update(GameTime gt)
        {
            Velocity += Acceleration;
            Velocity += _force;
            _force = Vector2.Zero;
            Velocity *= Drag;
            AngularVelocity *= AngularVelocityDrag;

            GetLink<Body>(DEPENDENCY_BODY).Position += Velocity;
            GetLink<Body>(DEPENDENCY_BODY).Angle += AngularVelocity;
        }

        public void Thrust(float power)
        {
            var angle = GetLink<Body>(DEPENDENCY_BODY).Angle;
            Thrust(power, angle);
        }

        public void Thrust(float power, float angle)
        {
            Velocity.X -= (float)Math.Sin(-angle) * power;
            Velocity.Y -= (float)Math.Cos(-angle) * power;
        }

        public void FaceVelocity()
        {
            GetLink<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(Velocity.X, -Velocity.Y);
        }

        public void FaceVelocity(Vector2 velocity)
        {
            GetLink<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(velocity.X, velocity.Y);
        }

        public void AddForce(Vector2 force)
        {
            _force += force;
        }

        public Physics Clone()
        {
            Physics p = new Physics(Parent, Name);
            p.AngularVelocity = AngularVelocity;
            p.AngularVelocityDrag = AngularVelocityDrag;
            p.Drag = Drag;
            p.Velocity = Velocity;
            p.Acceleration = Acceleration;
            p.Link(DEPENDENCY_BODY, GetLink(DEPENDENCY_BODY));
            return p;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}