using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityState : HashSet<Entity>, IComponent
    {
        public EntityGame Parent { get; private set; }

        public event Entity.EventHandler EntityAdded, EntityRemoved;

        public delegate void EventHandler(string name);

        public event EventHandler ChangeState;

        public string Name { get; private set; }

        public uint Id { get; private set; }

        public bool Default { get; private set; }

        public bool Active { get; private set; }

        public bool Visible { get; private set; }

        public uint LastId { get; private set; }

        public bool Debug { get; set; }

        protected bool Destroyed;

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

        public bool CheckEntity<T>(int id) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Id == id);
            return result != null;
        }

        public bool CheckEntity<T>(string name) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Name == name);
            return result != null;
        }

        public T GetService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            if (result == null)
                throw new Exception("Service " + typeof(T) + " does not exist!");
            return (T)result;
        }

        public bool CheckService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            return result != null;
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
            {
                ChangeState(name);
                //Figure out if the change was successful
                if (Parent.CurrentState.Name == Name)
                {
                    //It was not log it
                    EntityGame.Log.Write("Could not find " + name + "in the ChangeState!", this, Alert.Warning);
                }
                else
                {
                    EntityGame.Log.Write("Changed to state " + name, this, Alert.Info);
                }
            }
        }

        public virtual void Show(string name)
        {
            if (name == Name)
                Show();

            EntityGame.Log.Write("Shown" ,this, Alert.Info);
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
            //TODO: Find a fix for destroying this!
            if (Destroyed) 
                return;
            foreach (var service in Services)
            {
                service.Update(gt);
            }

            foreach (var entity in this.ToArray().Where(e => e.Active))
            {
                entity.Update(gt);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Destroyed) return;
            foreach (var service in Services)
            {
                service.Draw(sb);
            }

            foreach (var entity in this.ToArray().Where(e => e.Visible))
            {
                entity.Draw(sb);
            }
        }

        public virtual void Destroy(IComponent i = null)
        {
            foreach (var entity in this.ToArray())
            {
                entity.Destroy();
            }

            foreach (var service in Services.ToArray())
            {
                service.Destroy();
            }
            Destroyed = true;

            //Start off with a fresh camera.
            Camera c = new Camera();
            c.View();

            EntityGame.Log.Write("Destoyed", this, Alert.Info);
        }

        public void AddEntity(Entity e)
        {
            Add(e);
            e.DestroyEvent += RemoveEntity;
            if (EntityAdded != null)
                EntityAdded(e);

            EntityGame.Log.Write("Entity " + e.Name + " added with ID" + e.Id, this, Alert.Info);
        }

        public void RemoveEntity(Entity e)
        {
            Remove(e);
            if (EntityRemoved != null)
                EntityRemoved(e);
            EntityGame.Log.Write("Entity " + e.Name + " removed with ID" + e.Id, this, Alert.Info);
        }

        public uint GetId()
        {
            return LastId++;
        }
    }
}