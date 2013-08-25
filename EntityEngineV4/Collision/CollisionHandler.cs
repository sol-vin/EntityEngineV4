using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    /// <summary>
    /// A state-side service for handling collisions and dealing with the resolution of those collisions.
    /// </summary>
    public class CollisionHandler : Service
    {
        //TODO: Fix AABBvsAABB collision bug
        public const int OVERLAP_BIAS = 4;
        public const int NONE = 0x0;
        public const int UP = 0x1;
        public const int DOWN = 0x10;
        public const int LEFT = 0x100;
        public const int RIGHT = 0x1000;
        public const int ALL = 0x1111;

        /// <summary>
        /// List of colliding members on this state.
        /// </summary>
        private List<Collision> _collideables;

        /// <summary>
        /// Pairs to be sent in for testing.
        /// </summary>
        private HashSet<Pair> _pairs;

        /// <summary>
        /// The pairs that have already collided and generated a manifold as a result.
        /// </summary>
        private HashSet<Manifold> _manifolds;

        public CollisionHandler(EntityState stateref)
            : base(stateref, "CollisionHandler")
        {
            _collideables = new List<Collision>();
            _pairs = new HashSet<Pair>();
            _manifolds = new HashSet<Manifold>();
        }

        public override void Update(GameTime gt)
        {
            BroadPhase();
            foreach (var manifold in _manifolds)
            {
                manifold.A.OnCollision(manifold.B);
                manifold.B.OnCollision(manifold.A);

                //TODO: Fix Resolution!

                if (CanObjectsResolve(manifold.A, manifold.B) || CanObjectsResolve(manifold.B, manifold.A))
                {
                    //TestAABBvsAABBResolveCollision(manifold);
                    ResolveCollision(manifold);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            _manifolds.Clear();
            foreach (var collideable in _collideables)
            {
                collideable.CollisionDirection.Mask = NONE;
            }
        }

        public void AddCollision(Collision c)
        {
            //Check if the Collision is already in the list.
            if (Enumerable.Contains(_collideables, c)) return;
            _collideables.Add(c);

            //Generate our pairs
            ReconfigurePairs(c);
        }

        public void RemoveCollision(Collision c)
        {
            if (!Enumerable.Contains(_collideables, c)) return;
            _collideables.Remove(c);

            _pairs.RemoveWhere(pair => pair.A.Equals(c) || pair.B.Equals(c));
        }

        /// <summary>
        /// Reconfigures the pairs for a Collision c
        /// </summary>
        /// <param name="c">A collision.</param>
        public void ReconfigurePairs(Collision c)
        {
            //Remove pairs with this collision in it
            foreach (var pair in _pairs.ToArray().Where(pair => pair.A.Equals(c) || pair.B.Equals(c)))
            {
                _pairs.Remove(pair);
            }

            //Recalculate pairs with this new collision
            foreach (var other in _collideables)
            {
                if (c.Equals(other)) continue;
                if (CanObjectsPair(c, other))
                {
                    var p = new Pair(c, other);
                    _pairs.Add(p);
                }
            }
        }

        /// <summary>
        /// Generates the pairs used for testing collision.
        /// </summary>
        public void GeneratePairs()
        {
            if (_collideables.Count() <= 1) return;

            _pairs.Clear();

            foreach (var a in _collideables)
            {
                foreach (var b in _collideables)
                {
                    if (a.Equals(b)) continue;
                    if (CanObjectsPair(a, b))
                    {
                        var p = new Pair(a, b);
                        _pairs.Add(p);
                    }
                }
            }
        }

        public void BroadPhase()
        {
            //Do a basic SAT test
            foreach (var pair in _pairs)
            {
                Manifold m = AABBvsAABB(AABB.CreateAABB(pair.A.BoundingRect, pair.A), AABB.CreateAABB(pair.B.BoundingRect, pair.B));
                if (m.AreColliding)
                {
                    //Do our real test now.
                    if (pair.A.Shape is AABB && pair.B.Shape is AABB)
                        //If the shapes are both AABB's, skip the check, we already have it
                        _manifolds.Add(m);
                    else
                    {
                        m = CheckCollision(pair.A.Shape, pair.B.Shape);
                        if (m.AreColliding)
                            _manifolds.Add(m);
                    }
                }
            }
        }

        //Static methods

        /// <summary>
        /// Compares the masks and checks to see if they should be allowed to form a pair.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Whether or not the the two objects should be paired</returns>
        public static bool CanObjectsPair(Collision a, Collision b)
        {
            return (a.GroupMask.HasMatchingBit(b.PairMask) || //Compare the pair masks to the group masks.
                    a.PairMask.HasMatchingBit(b.GroupMask)) && a.Enabled && b.Enabled;
        }

        public static bool CanObjectsResolve(Collision resolver, Collision other)
        {
            return resolver.ResolutionGroupMask.HasMatchingBit(other.ResolutionGroupMask) //Compare the pair mask one sided.
                && resolver.Enabled && other.Enabled && !resolver.Immovable;
        }

        public static void ResolveCollision(Manifold m)
        {
            Vector2 relVelocity = m.B.PositionDelta - m.A.PositionDelta;
            //Finds out if the objects are moving towards each other.
            //We only need to resolve collisions that are moving towards, not away.
            float velAlongNormal = MathTools.Physics.DotProduct(relVelocity, m.Normal);
            if (velAlongNormal > 0)
                return;
            float e = Math.Min(m.A.Restitution, m.B.Restitution);

            float j = -(1 + e) * velAlongNormal;
            j /= m.A.InvertedMass + m.B.InvertedMass;

            Vector2 impulse = j * m.Normal;
            if (CanObjectsResolve(m.A, m.B))
                m.A.Velocity -= m.A.InvertedMass * impulse;
            if (CanObjectsResolve(m.B, m.A))
                m.B.Velocity += m.B.InvertedMass * impulse;
        }

        public static void PositionalCorrection(Manifold m)
        {
            const float percent = 0.2f;
            const float slop = 0.01f;
            Vector2 correction = Math.Max(m.PenetrationDepth - slop, 0.0f) / (m.A.InvertedMass + m.B.InvertedMass) * percent * m.Normal;
            if (CanObjectsResolve(m.A, m.B))
                m.A.Position -= m.A.InvertedMass * correction;
            if (CanObjectsResolve(m.B, m.A))
                m.B.Position += m.B.InvertedMass * correction;
        }

        public static Manifold TestAABBvsAABB(AABB a, AABB b)
        {
            var m = new Manifold(a, b);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares bounding boxes using Seperating Axis Thereom.
        /// </summary>
        public static Manifold AABBvsAABB(AABB a, AABB b)
        {
            //Start packing the manifold
            var m = new Manifold(a, b);
            m.Normal = a.Position - b.Position;

            //Calculate half widths
            float aExtent = a.Width / 2f;
            float bExtent = b.Width / 2f;

            //Calculate the overlap.
            float xExtent = aExtent + bExtent - Math.Abs(m.Normal.X);

            //If the overlap is greater than 0
            if (xExtent > 0)
            {
                //Calculate half widths
                aExtent = a.Height / 2f;
                bExtent = b.Height / 2f;

                //Calculate overlap
                float yExtent = aExtent + bExtent - Math.Abs(m.Normal.Y);

                if (yExtent > 0)
                {
                    //Variable to multiply the normal by to make the collision resolve
                    Vector2 fixnormal;

                    //Check to see which axis has the biggest "penetration" ;D

                    //Collision is happening on Y axis
                    if (xExtent > yExtent)
                    {
                        if (m.Normal.X < 0)
                            fixnormal = -Vector2.UnitX;
                        else
                            fixnormal = Vector2.UnitX;

                        if (m.B.BoundingRect.Top > m.A.BoundingRect.Top && m.A.BoundingRect.Top < m.B.BoundingRect.Bottom)
                        {
                            if (m.A.AllowCollisionDirection.HasMatchingBit(DOWN))
                                m.A.CollisionDirection.CombineMask(DOWN);
                            if (m.B.AllowCollisionDirection.HasMatchingBit(UP))
                                m.B.CollisionDirection.CombineMask(UP);
                        }
                        else if (m.A.BoundingRect.Top > m.B.BoundingRect.Top && m.B.BoundingRect.Top < m.A.BoundingRect.Bottom)
                        {
                            if (m.A.AllowCollisionDirection.HasMatchingBit(UP))
                                m.A.CollisionDirection.CombineMask(UP);
                            if (m.B.AllowCollisionDirection.HasMatchingBit(DOWN))
                                m.B.CollisionDirection.CombineMask(DOWN);
                        }

                        m.Normal = MathTools.Physics.GetNormal(a.Position, b.Position) * fixnormal.X;
                        m.PenetrationDepth = xExtent;
                    }
                    //Collision happening on X axis
                    else
                    {
                        if (m.Normal.Y < 0)
                            fixnormal = -Vector2.UnitY;
                        else
                            fixnormal = Vector2.UnitY;
                        if (m.B.BoundingRect.Left > m.A.BoundingRect.Left &&
                            m.A.BoundingRect.Left < m.B.BoundingRect.Right)
                        {
                            if (m.A.AllowCollisionDirection.HasMatchingBit(RIGHT))
                                m.A.CollisionDirection.CombineMask(RIGHT);
                            if (m.B.AllowCollisionDirection.HasMatchingBit(LEFT))
                                m.B.CollisionDirection.CombineMask(LEFT);
                        }
                        else if (m.A.BoundingRect.Left > m.B.BoundingRect.Left &&
                            m.B.BoundingRect.Left < m.A.BoundingRect.Right)
                        {
                            if (m.A.AllowCollisionDirection.HasMatchingBit(LEFT))
                                m.A.CollisionDirection.CombineMask(LEFT);
                            if (m.B.AllowCollisionDirection.HasMatchingBit(RIGHT))
                                m.B.CollisionDirection.CombineMask(RIGHT);
                        }

                        m.Normal = MathTools.Physics.GetNormal(a.Position, b.Position) * fixnormal.Y;
                        m.PenetrationDepth = yExtent;
                    }
                    //Check to see if any flags actually got toggled.
                    //If they didn't then, ensure that the manifold reports they don't collide.
                    if (m.A.AllowCollisionDirection == 0 ||
                        m.B.AllowCollisionDirection == 0 ||
                        m.B.CollisionDirection == 0 ||
                        m.A.CollisionDirection == 0)
                        m.AreColliding = false;
                    else
                        m.AreColliding = true;
                    return m;
                }
            }
            m.AreColliding = false;
            return m;
        }

        //Collision resolver methods
        /// <summary>
        /// Uses a table to test collision between various shapes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Manifold CheckCollision(Shape a, Shape b)
        {
            if (a is AABB && b is AABB)
                return AABBvsAABB((AABB)a, (AABB)b);

            throw new Exception("No existing methods for this kind of collision!");
        }
    }
}