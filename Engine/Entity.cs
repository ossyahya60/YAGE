using System;
using System.Collections;

namespace DataOrientedEngine.Engine
{
    // This is a container of components
    public class Entity
    {
        public int ID;
        public string Name;
        public bool Active;

        internal bool Destroyed;

        private BitArray Components;

        public Entity()
        {
            Components = new BitArray((Enum.GetValues(typeof(Components)).Length));
            Active = true;
            Destroyed = false;
            Name = "Entity";
        }

        public Entity(string name)
        {
            Components = new BitArray((Enum.GetValues(typeof(Components)).Length));
            Active = true;
            Destroyed = false;
            Name = name;
        }

        public void AddComponent(Component component)
        {
            if (component.GetType().Name == "Component")
                throw new Exception("You can't add a base component");

            Components.Set((int)component.ComponentID, true);
        }

        public void RemoveComponent(Component component)
        {
            if (component.GetType().Name == "Component")
                throw new Exception("You can't remove a base component");

            Components.Set((int)component.ComponentID, false);
        }

        // Remove a component by type
        public void RemoveComponent(Components componentType)
        {
            Components.Set((int)componentType, false);
        }

        public bool HasComponent(Components componentType)
        {
            return Components[(int)componentType];
        }

        public void Destroy() // Figure out a way to destroy all components
        {
            Destroyed = true;

            
        }
    }
}
