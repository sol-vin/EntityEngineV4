using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Entity : List<IComponent>, IComponent
    {
        public IComponent Parent { get; private set; }

        public EntityState StateRef { get; private set; }

        public delegate void EventHandler(Entity e);

        public event EventHandler DestroyEvent;

        public event Component.EventHandler ComponentAdded, ComponentRemoved;

        public string Name { get; protected set; }

        public uint Id { get; private set; }

        public bool Default { get; set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public bool Debug { get; set; }

        public Entity(EntityState stateref, IComponent parent, string name)
        {
            StateRef = stateref;
            Parent = parent;
            Name = name;
            Id = StateRef.GetId();
            Active = true;
            Visible = true;
        }

        public Entity(EntityState stateref, string name)
        {
            StateRef = stateref;
            Parent = stateref;
            Id = StateRef.GetId();
            Name = name;
            Active = true;
            Visible = true;
        }

        public virtual void Update(GameTime gt)
        {
            foreach (var component in ToArray().Where(c => c.Active))
            {
                component.Update(gt);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (var component in ToArray().Where(c => c.Visible))
            {
                component.Draw(sb);
            }
        }

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            foreach (var component in ToArray())
            {
                component.Destroy();
            }

            EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
        }

        public T GetComponent<T>(string name) where T : IComponent
        {
            if (name == "") return GetComponent<T>();
            var result = this.FirstOrDefault(c => c.Name == name);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + name + " does not exist", this, Alert.Warning);
                throw new Exception("Component " + name + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public T GetComponent<T>(int id) where T : IComponent
        {
            var result = this.FirstOrDefault(c => c.Id == id);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + id + " does not exist", this, Alert.Warning);
                throw new Exception("Component ID:" + id + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public T GetComponent<T>() where T : IComponent
        {
            var result = this.FirstOrDefault(c => c is T);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + typeof(T) + " does not exist", this, Alert.Warning);
                throw new Exception("Component of type " + typeof(T) + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public void AddComponent(Component c)
        {
            if (this.Any(component => c.Name == component.Name))
            {
                EntityGame.Log.Write("Component " + c.Name + " already exists", this, Alert.Warning);
                throw new Exception(c.Name + " already exists in " + Name + "\'s list!");
            }

            Add(c);

            if (ComponentAdded != null)
                ComponentAdded(c);
        }

        public void RemoveComponent(Component c)
        {
            Remove(c);
            if (ComponentRemoved != null)
                ComponentRemoved(c);
        }
    }
}