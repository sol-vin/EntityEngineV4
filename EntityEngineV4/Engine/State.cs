using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class State : Node
    {
        public override bool IsRoot
        {
            get { return true; }
        }
        public event Timer.TimerEvent PreUpdateEvent;

        public event Service.EventHandler ServiceAdded;
        public event Service.EventHandler ServiceRemoved;

        public delegate void EventHandler(State state);
        public event EventHandler ShownEvent;

        public List<Service> Services;

        public enum InitializeAction
        {
            OncePerShow, OnceInLife
        }

        public InitializeAction InitializeActionOnShow = InitializeAction.OnceInLife;

        public State(string name) : base(null, name)
        {
            Services = new List<Service>();

            Active = true;
            Visible = true;

            //Keep track of all nodes being added/removed, state will know because all NodeAdded/Rmoved events trickle 
            //up the parent chain to root.
            NodeAdded += OnNodeAdded;
            NodeRemoved += OnNodeRemoved;
        }

        private void OnNodeRemoved(Node node)
        {
            ActiveNodes--;
        }

        private void OnNodeAdded(Node node)
        {
            ActiveNodes++;
        }

        public override bool RemoveChild(Node node)
        {
            Service s = node as Service;
            if (s != null)
            {
                Services.Remove(s);
                if (ServiceRemoved != null) ServiceRemoved(s);
            }
            return base.RemoveChild(node);
        }

        public override void AddChild(Node node)
        {
            base.AddChild(node);

            Service s = node as Service;
            if (s != null)
            {
                Services.Add(s);
                if (ServiceAdded != null) ServiceAdded(s);
            }
        }

        public int ActiveNodes { get; private set; }


        public T GetService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            if (result == null)
            {
                EntityGame.Log.Write("Service " + typeof(T) + " does not exist!", this, Alert.Warning);
                return null;
            }
            return (T)result;
        }

        public Service GetService(Type t)
        {
            var result = Services.FirstOrDefault(service => service.GetType() == t);
            if (result == null)
            {
                EntityGame.Log.Write("Service " + t + " does not exist!", this, Alert.Warning);
                return null;
            }
            return result;
        }

        public bool CheckService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            return result != null;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public virtual void Show()
        {
            EntityGame.ActiveState = this;

            if (ShownEvent != null)
                ShownEvent(this);

            switch (InitializeActionOnShow)
            {
                case InitializeAction.OnceInLife:
                    if(!Initialized) Initialize();
                    break;
                case InitializeAction.OncePerShow:
                    Initialize();
                    break;

            }

            EntityGame.Log.Write("Shown", this, Alert.Info);
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var child in Children.ToArray())
            {
                child.Destroy(this);
            }
            Services.Clear();
        }

        public virtual void PreUpdate()
        {
            if (PreUpdateEvent != null) PreUpdateEvent();
        }

        public virtual void Update(GameTime gt)
        {
            if(!Initialized) Initialize();
            if (Destroyed) return;

            PreUpdate();

            //foreach (var service in Services.ToArray().Where(s => s.Active))
            //{
            //    service.Update(gt);
            //}

            //TODO: Replace with update service
            UpdateChildren(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //foreach (var service in Services.ToArray().Where(s => s.Active))
            //{
            //    service.Draw(sb);
            //}

            //TODO: Replace with draw service
            DrawChildren(sb);
        }

        public override void Destroy(IComponent sender = null)
        {
            base.Destroy(sender);
            Reset();

            //Start off with a fresh camera.
            Camera c = new Camera(this, Name + ".Camera");
            c.View();

            EntityGame.Log.Write("Destoyed", this, Alert.Info);
        }
    }
}