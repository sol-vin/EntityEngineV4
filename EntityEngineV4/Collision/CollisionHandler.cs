using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    /// <summary>
    /// A state-side service for handling collisions and dealing with the resolution of those collisions.
    /// </summary>
    public class CollisionHandler : Service
    {
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
                //Attempt to resolve collisions
                //if (CanObjectsResolve(manifold.A, manifold.B) || CanObjectsResolve(manifold.B, manifold.A))
                //{
                //    TestAABBvsAABBResolveCollision(manifold);
                //    PositionalCorrection(manifold);
                //}
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
            float velAlongNormal = PhysicsMath.DotProduct(relVelocity, m.Normal);
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

        public static void TestAABBvsAABBResolveCollisionX(Manifold m)
        {
            //Can't seperate immovable objects
            if (m.A.Immovable && m.B.Immovable) return;

            float overlap = 0;
            float ADelta = m.A.Delta.X;
            float BDelta = m.B.Delta.X;

            if (Math.Abs(ADelta - BDelta) > .01f) //If A.Delta and B.Delta are not equal
            {
                Rectangle ARect = new Rectangle(
                    (int)(m.A.BoundingRect.X - ((ADelta > 0)?ADelta:0)),
                    (int)m.A.LastPosition.Y,
                    m.A.BoundingRect.Width + (int)Math.Abs(ADelta),
                    m.A.BoundingRect.Height);

                Rectangle BRect = new Rectangle(
                    (int)(m.B.BoundingRect.X - ((BDelta > 0) ? BDelta : 0)),
                    (int)m.B.LastPosition.Y,
                    m.B.BoundingRect.Width + (int)Math.Abs(BDelta),
                    m.B.BoundingRect.Height);

                //Find the overlaps
                if (ARect.Right > BRect.Left &&
                    ARect.Left < BRect.Right &&
                    ARect.Bottom > BRect.Top &&
                    ARect.Top < BRect.Bottom)
                {
                    float maxoverlap = Math.Abs(ADelta) + Math.Abs(BDelta) + OVERLAP_BIAS;
                    overlap = m.A.BoundingRect.Right - m.B.BoundingRect.Left;
                    if (ADelta > BDelta)
                    {
                        if (overlap > maxoverlap || !m.A.AllowCollisionDirection.HasMatchingBit(RIGHT) || !m.B.AllowCollisionDirection.HasMatchingBit(LEFT))
                            overlap = 0;
                        else
                        {
                            m.A.CollisionDirection.CombineMask(RIGHT);
                            m.B.CollisionDirection.CombineMask(LEFT);
                        }
                    }
                    else if (ADelta < BDelta)
                    {
                        if (-overlap > maxoverlap || !m.A.AllowCollisionDirection.HasMatchingBit(LEFT) || !m.B.AllowCollisionDirection.HasMatchingBit(RIGHT))
                            overlap = 0;
                        else
                        {
                            m.A.CollisionDirection.CombineMask(LEFT);
                            m.B.CollisionDirection.CombineMask(RIGHT);
                        }
                    }
                }
            }

            //Begin resolving the collision
            if (Math.Abs(overlap) > 0.0001)
            {
                float Av = m.A.Velocity.X;
                float Bv = m.B.Velocity.X;



                if (!m.A.Immovable && !m.B.Immovable)
                {
                    overlap *= .5f;
                    m.A.Position = new Vector2(m.A.Position.X - overlap, m.A.Position.Y);
                    m.B.Position = new Vector2(m.B.Position.X + overlap, m.A.Position.Y);

                    float AVelocity = (float)Math.Sqrt(((Bv * Bv * m.B.Mass)/m.A.Mass) * ((Bv > 0) ? 1: -1));
                    float BVelocity = (float)Math.Sqrt(((Av * Av * m.A.Mass) / m.B.Mass) * ((Av > 0) ? 1 : -1));

                    float average = (AVelocity + BVelocity)*.5f;
                    AVelocity -= average;
                    BVelocity -= average;
                    m.A.Velocity = new Vector2(AVelocity * m.A.Restitution, m.A.Velocity.Y);
                    m.B.Velocity = new Vector2(BVelocity * m.B.Restitution, m.A.Velocity.Y);
                }
                else if (!m.A.Immovable)
                {
                    m.A.Position = new Vector2(m.A.Position.X - overlap, m.A.Position.Y);
                    m.A.Velocity = new Vector2(Bv-Av*m.A.Restitution, m.A.Velocity.Y);
                }
                else if (!m.B.Immovable)
                {
                    m.B.Position = new Vector2(m.B.Position.X + overlap, m.B.Position.Y);
                    m.B.Velocity = new Vector2(Av - Bv * m.B.Restitution, m.A.Velocity.Y);
                }
            }
        }

        public static void TestAABBvsAABBResolveCollisionY(Manifold m)
        {
            //Can't seperate immovable objects
            if (m.A.Immovable && m.B.Immovable) return;

            float overlap = 0;
            float ADelta = m.A.Delta.Y;
            float BDelta = m.B.Delta.Y;
            if (Math.Abs(ADelta - BDelta) > 0.001)
            {
                Rectangle ARect = new Rectangle(
                    (int) m.A.LastPosition.X,
                    (int)(m.A.Position.Y - ((ADelta > 0) ?ADelta : 0)),
                    m.A.BoundingRect.Width,
                    m.A.BoundingRect.Height + (int)Math.Abs(ADelta) );

                Rectangle BRect = new Rectangle(
                    (int)m.B.LastPosition.X,
                    (int)(m.B.Position.Y - ((BDelta > 0) ?BDelta : 0)),
                    m.B.BoundingRect.Width,
                    m.B.BoundingRect.Height + (int)Math.Abs(BDelta));

                //Find the overlaps
                if (ARect.Right > BRect.Left &&
                    ARect.Left < BRect.Right &&
                    ARect.Bottom > BRect.Top &&
                    ARect.Top < BRect.Bottom)
                {
                    float maxoverlap = Math.Abs(ADelta) + Math.Abs(BDelta) + OVERLAP_BIAS;
                    overlap = m.A.BoundingRect.Bottom - m.B.BoundingRect.Top;
                    if (m.A.Delta.Y > BDelta)
                    {
                        if (overlap > maxoverlap || !m.A.AllowCollisionDirection.HasMatchingBit(DOWN) || !m.B.AllowCollisionDirection.HasMatchingBit(UP))
                            overlap = 0;
                        else
                        {
                            m.A.CollisionDirection.CombineMask(DOWN);
                            m.B.CollisionDirection.CombineMask(UP);
                        }
                    }
                    else if (m.A.Delta.Y < BDelta)
                    {
                        if (-overlap > maxoverlap || !m.A.AllowCollisionDirection.HasMatchingBit(UP) || !m.B.AllowCollisionDirection.HasMatchingBit(DOWN))
                            overlap = 0;
                        else
                        {
                            m.A.CollisionDirection.CombineMask(UP);
                            m.B.CollisionDirection.CombineMask(DOWN);
                        }
                    }
                }
            }

            //Begin resolving the collision
            if (Math.Abs(overlap) > 0.0001)
            {
                float Av = m.A.Velocity.Y;
                float Bv = m.B.Velocity.Y;

                if (!m.A.Immovable && !m.B.Immovable)
                {
                    overlap *= .5f;
                    m.A.Position = new Vector2(m.A.Position.X, m.A.Position.Y - overlap);
                    m.B.Position = new Vector2(m.A.Position.X, m.B.Position.Y + overlap);

                    float AVelocity = (float)Math.Sqrt(((Bv * Bv * m.B.Mass)/m.A.Mass) * ((Bv > 0) ? 1 : -1));
                    float BVelocity = (float)Math.Sqrt(((Av * Av * m.A.Mass) / m.B.Mass) * ((Av > 0) ? 1 : -1));

                    float average = (AVelocity + BVelocity) * .5f;
                    AVelocity -= average;
                    BVelocity -= average;
                    m.A.Velocity = new Vector2(m.A.Position.X, AVelocity * m.A.Restitution);
                    m.B.Velocity = new Vector2(m.A.Position.X, BVelocity * m.B.Restitution);
                }
                else if (!m.A.Immovable)
                {
                    m.A.Position = new Vector2(m.A.Position.X, m.A.Position.Y - overlap);
                    m.A.Velocity = new Vector2(m.A.Position.X, Bv - Av * m.A.Restitution);
                }
                else if (!m.B.Immovable)
                {
                    m.B.Position = new Vector2(m.A.Position.X, m.B.Position.Y + overlap);
                    m.B.Velocity = new Vector2(m.A.Position.X, Av - Bv * m.B.Restitution);
                }
            }
        }

        public static void TestAABBvsAABBResolveCollision(Manifold m)
        {
            TestAABBvsAABBResolveCollisionX(m);
            TestAABBvsAABBResolveCollisionY(m);
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

        /// <summary>
        /// Compares bounding boxes using Seperating Axis Thereom.
        /// </summary>
        public static Manifold AABBvsAABB(AABB a, AABB b)
        {
            //Start packing the manifold
            Manifold m = new Manifold(a.Collision, b.Collision);
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


                        m.Normal = PhysicsMath.GetNormal(a.Position, b.Position) * fixnormal.X;
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

                        m.Normal = PhysicsMath.GetNormal(a.Position, b.Position) * fixnormal.Y;
                        m.PenetrationDepth = yExtent;
                    }

                    m.AreColliding = true;
                    return m;
                }
            }
            m.AreColliding = false;
            return m;
        }

        //Collision resolver methods

        public static Manifold CheckCollision(Shape a, Shape b)
        {
            if (a is AABB && b is AABB)
                return AABBvsAABB((AABB)a, (AABB)b);

            throw new Exception("No existing methods for this kind of collision!");
        }
    }
}