using System;
using System.Collections.Generic;

namespace EntityEngineV4.Engine
{
    public class Component : Node
    {
        public delegate void EventHandler(Component i);

        public Component(Node parent, string name)
            : base(parent, name)
        {
            GetRoot<State>().PreUpdateEvent += SubscribePreUpdate;

            CreateDependencyList();
        }

        private void SubscribePreUpdate()
        {
            //Initialize
            if (!Initialized && !Destroyed) Initialize();
            GetRoot<State>().PreUpdateEvent -= SubscribePreUpdate; //Unsubscribe
        }

        //Dependencies

        /// <summary>
        /// List of links to this component
        /// </summary>
        private Dictionary<int, Component> _links = new Dictionary<int, Component>();

        /// <summary>
        /// The last component which was grabbed using GetDependency. Useful for faster returns on the same object
        /// </summary>
        private Component _cachedLink;

        /// <summary>
        /// The last index accessed using GetDependency. Useful for faster returns on the same object
        /// </summary>
        private int _cachedIndex = -1;

        /// <summary>
        /// Links a component to this component
        /// </summary>
        /// <param name="index"></param>
        /// <param name="component"></param>
        public void LinkDependency(int index, Component component)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            //First check if the type of component is the expected type
            if (!(component.GetType() == GetExpectedDependencyType(index) ||
                component.GetType().IsSubclassOf(GetExpectedDependencyType(index))))
            {
                throw new Exception("Component type " + component.GetType() + " is not of expected type " + GetExpectedDependencyType(index));
            }

            //It passed so now we add the link
            _links.Add(index, component);
        }

        /// <summary>
        /// Unlinks a component
        /// </summary>
        /// <param name="index"></param>
        public void Unlink(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            _links.Remove(index);
        }

        /// <summary>
        /// Gets the link of a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetDependency<T>(int index) where T : Component
        {
            if (index < 0) throw new IndexOutOfRangeException();
            //Check our link cache
            if (index == _cachedIndex) return _cachedLink as T;

            var component = _links[index] as T;
            if (component == null) throw new Exception("Component does not exist or is not of this type");

            //Change out cache
            _cachedLink = component;
            _cachedIndex = index;
            return component;
        }

        public Component GetDependency(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            //Check our link cache
            if (index == _cachedIndex) return _cachedLink;

            var component = _links[index];
            if (component == null) throw new Exception("Component does not exist or is not of this type");

            //Change out cache
            _cachedLink = component;
            _cachedIndex = index;
            return component;
        }

        /// <summary>
        /// Holds the expected types for each parameter
        /// </summary>
        private Dictionary<int, Type> _linkTypes = new Dictionary<int, Type>();

        /// <summary>
        /// Adds a new possible link to the component
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        protected void AddLinkType(int index, Type type)
        {
            _linkTypes.Add(index, type);
        }

        /// <summary>
        /// Removes a possible link for a component
        /// </summary>
        /// <param name="index"></param>
        protected bool RemoveLinkType(int index)
        {
            return _linkTypes.Remove(index);
        }

        public Type GetExpectedDependencyType(int index)
        {
            if (_linkTypes[index] == null) throw new IndexOutOfRangeException();
            return _linkTypes[index];
        }

        public virtual void CreateDependencyList()
        {
        }
    }
}