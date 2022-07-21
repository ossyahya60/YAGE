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
        public int[] componentIndex;

        internal bool Destroyed;

        public Entity()
        {
            Active = true;
            Destroyed = false;
            Name = "Entity";
            componentIndex = new int[Enum.GetValues(typeof(Components)).Length];

            for (int i = 0; i < componentIndex.Length; i++)
                componentIndex[i] = -1;
        }

        public Entity(string name)
        {
            Active = true;
            Destroyed = false;
            Name = name;
            componentIndex = new int[Enum.GetValues(typeof(Components)).Length];

            for (int i = 0; i < componentIndex.Length; i++)
                componentIndex[i] = -1;
        }

        public bool HasComponent(Components componentType)
        {
            return componentIndex[(int)componentType] != -1;
        }

        public void Destroy() // Figure out a way to destroy all components
        {
            Destroyed = true;
        }
    }
}
