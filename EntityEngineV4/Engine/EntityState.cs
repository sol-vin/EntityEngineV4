using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityState : HashSet<IComponent>, IComponent
    {
        public IComponent Parent { get; private set; }
        public EntityGame GameRef { get { return Parent as EntityGame; } }

        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;
        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;
        public event Service.EventHandler AddServiceEvent, RemoveServiceEvent;
        public event Engine.EventHandler DestroyEvent;

        public delegate void EventHandler(string name);
        public event EventHandler ShownEvent;
        public event EventHandler ChangeState;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public uint LastId { get; private set; }
        public bool Debug { get; set; }

        protected bool Destroyed;

        public List<Service> Services;

        public EntityState()
        {
            Parent = EntityGame.Self;

            Services = new List<Service>();
            ShownEvent += s => Create();

            Active = true;
            Visible = true;

            Id = EntityGame.GetID();
        }

        public EntityState(string name)
        {
            Parent = EntityGame.Self;
            Name = name;

            Services = new List<Service>();
            ShownEvent += s => Create();

            Active = true;
            Visible = true;

            Id = EntityGame.GetID();
        }

        public T GetEntity<T>(string name) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Name == name);
            if (result == null)
            {
                EntityGame.Log.Write("Entity " + name + " does not exist!", this, Alert.Warning);
                throw new Exception("Entity " + name + " does not exist!");
            }
            return (T)result;
        }

        public T GetEntity<T>(int id) where T : Entity
        {
            var result = this.FirstOrDefault(entity => entity.Id == id);
            if (result == null)
            {
                EntityGame.Log.Write("Entity " + id + " does not exist!", this, Alert.Warning);
                throw new Exception("Entity ID " + id + " does not exist!");
            }
            return (T)result;
        }

        public bool CheckEntity(int id)
        {
            var result = this.FirstOrDefault(entity => entity.Id == id);
            return result != null;
        }

        public bool CheckEntity(string name)
        {
            var result = this.FirstOrDefault(entity => entity.Name == name);
            return result != null;
        }

        public T GetService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            if (result == null)
            {
                EntityGame.Log.Write("Service " + typeof(T) + " does not exist!", this, Alert.Warning);
                throw new Exception("Service " + typeof(T) + " does not exist!");
            }
            return (T)result;
        }

        public bool CheckService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            return result != null;
        }

        //TODO: Create a type to regulate when this is called after a Show()
        public virtual void Create()
        {
        }

        public virtual void Show()
        {
            EntityGame.ActiveState = this;

            if (ShownEvent != null)
                ShownEvent(this.Name);

            EntityGame.Log.Write("Shown", this, Alert.Info);
        }

        public virtual void ChangeToState(string name)
        {
            if (ChangeState != null)
            {
                ChangeState(name);
                //Figure out if the change was successful
                if (EntityGame.ActiveState.Name == Name)
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
        }

        public virtual void Reset()
        {
            Clear();
            Services.Clear();
        }

        public virtual void Update(GameTime gt)
        {
            //TODO: Find a fix for destroying this!
            if (Destroyed) return;

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
            Destroyed = true;

            if (DestroyEvent != null)
                DestroyEvent(this);

            //Start off with a fresh camera.
            Camera c = new Camera(this, Name + ".Camera");
            c.View();

            EntityGame.Log.Write("Destoyed", this, Alert.Info);
        }

        public void AddEntity(Entity e)
        {
            Add(e);
            e.AddEntityEvent += AddEntity;
            e.RemoveEntityEvent += RemoveEntity;
            DestroyEvent += e.Destroy;

            if (AddEntityEvent != null)
                AddEntityEvent(e);

            EntityGame.Log.Write("Entity " + e.Name + " added with ID" + e.Id, this, Alert.Trivial);
        }

        public void RemoveEntity(Entity e)
        {
            Remove(e);
            e.AddEntityEvent -= AddEntity;
            e.RemoveEntityEvent -= RemoveEntity;
            DestroyEvent -= e.Destroy;

            if (RemoveEntityEvent != null)
                RemoveEntityEvent(e);
            EntityGame.Log.Write("Entity " + e.Name + " removed with ID" + e.Id, this, Alert.Trivial);
        }

        public uint GetId()
        {
            return LastId++;
        }

        public void AddComponent(Component c)
        {
            if (AddComponentEvent != null)
            {
                AddComponentEvent(c);
            }
            else
            {
                EntityGame.Log.Write("AddComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void RemoveComponent(Component c)
        {
            if (RemoveComponentEvent != null)
            {
                RemoveComponentEvent(c);
            }
            else
            {
                EntityGame.Log.Write("RemoveComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void AddService(Service s)
        {
            s.AddEntityEvent += AddEntity;
            s.RemoveEntityEvent += RemoveEntity;
            DestroyEvent += s.Destroy;

            Services.Add(s);
            if (AddServiceEvent != null)
            {
                AddServiceEvent(s);
            }

            EntityGame.Log.Write("Added service " + s.Name, this, Alert.Info);
        }

        public void RemoveService(Service s)
        {
            s.AddEntityEvent -= AddEntity;
            s.RemoveEntityEvent -= RemoveEntity;
            DestroyEvent -= s.Destroy;

            Services.Remove(s);
            if (RemoveServiceEvent != null)
            {
                RemoveServiceEvent(s);
            }

            EntityGame.Log.Write("Removed service " + s.Name, this, Alert.Info);
        }
    }
}