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

        private BitArray components;
        private int[] componentIndex;

        public Entity()
        {
            components = new BitArray(Enum.GetValues(typeof(Components)).Length);
            Active = true;
            Destroyed = false;
            Name = "Entity";
            componentIndex = new int[Enum.GetValues(typeof(Components)).Length];

            for (int i = 0; i < componentIndex.Length; i++)
                componentIndex[i] = -1;
        }

        public Entity(string name)
        {
            components = new BitArray(Enum.GetValues(typeof(Components)).Length);
            Active = true;
            Destroyed = false;
            Name = name;
            componentIndex = new int[Enum.GetValues(typeof(Components)).Length];

            for (int i = 0; i < componentIndex.Length; i++)
                componentIndex[i] = -1;
        }

        public void AddComponent(Component component)
        {
            if (component.GetType().Name == "Component")
                throw new Exception("You can't add a base component");

            if (componentIndex[(int)component.ComponentID] == -1)
                throw new Exception("Component already exists in the entity");

            //componentIndex[(int)component.ComponentID] = 

            //components.Set((int)component.ComponentID, true);
        }

        public void RemoveComponent(Component component)
        {
            if (component.GetType().Name == "Component")
                throw new Exception("You can't remove a base component");

            components.Set((int)component.ComponentID, false);
        }

        // Remove a component by type
        public void RemoveComponent(Components componentType)
        {
            components.Set((int)componentType, false);
        }

        public bool HasComponent(Components componentType)
        {
            return components[(int)componentType];
        }

        public void Destroy() // Figure out a way to destroy all components
        {
            Destroyed = true;

            
        }
    }
}
