using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityState : List<Entity>, IComponent
    {
        public EntityGame Parent { get; private set; }

        public event Entity.EventHandler EntityAdded, EntityRemoved;

        public delegate void EventHandler(string name);

        public event EventHandler ChangeState;

        public string Name { get; private set; }

        public int Id { get; private set; }

        public bool Default { get; private set; }

        public bool Active { get; private set; }

        public bool Visible { get; private set; }

        public int LastId { get; private set; }

        public List<Service> Services;

        public EntityState(EntityGame eg, string name)
        {
            Parent = eg;
            Name = name;

            Services = new List<Service>();
        }


        public T GetEntity<T>(string name) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Name == name);
            if (result == null)
                throw new Exception("Entity " + name + " does not exist!");
            return (T)result;
        }

        public T GetEntity<T>(int id) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Id == id);
            if (result == null)
                throw new Exception("Entity ID " + id + " does not exist!");
            return (T)result;
        }

        public virtual void Start()
        {
        }

        public virtual void Show()
        {
            Parent.CurrentState = this;
        }

        public virtual void ChangeToState(string name)
        {
            if (ChangeState != null)
                ChangeState(name);
            else
            {
                throw new Exception("Called" + Name + ".ChangeToState(" + name + ") while there were no states to change to i n ChangeState!");
            }
        }

        public virtual void Show(string name)
        {
            if (name == Name)
                Show();
        }

        public virtual void Hide()
        {
        }

        public virtual void Reset()
        {
            Clear();
            Services.Clear();
        }

        public virtual void Update(GameTime gt)
        {
            foreach (var service in Services)
            {
                service.Update(gt);
            }

            foreach (var entity in ToArray().Where(e => e.Active))
            {
                entity.Update(gt);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (var service in Services)
            {
                service.Draw(sb);
            }

            foreach (var entity in ToArray().Where(e => e.Visible))
            {
                entity.Draw(sb);
            }
        }

        public virtual void Destroy(IComponent i = null)
        {
            foreach (var entity in ToArray())
            {
                entity.Destroy();
            }

            foreach (var service in Services)
            {
                service.Destroy();
            }
        }

        public void AddEntity(Entity e)
        {
            Add(e);
            e.DestroyEvent += RemoveEntity;
            e.CreateEvent += AddEntity;
            if (EntityAdded != null)
                EntityAdded(e);
        }

        public void RemoveEntity(Entity e)
        {
            Remove(e);
            if (EntityRemoved != null)
                EntityRemoved(e);
        }

        public int GetId()
        {
            return LastId++;
        }
    }
}