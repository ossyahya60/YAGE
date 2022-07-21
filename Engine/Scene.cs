using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework.Content;

namespace DataOrientedEngine.Engine
{
    public class Scene
    {
        public const int MAX_ENTITIES = 1000;

        public static Scene ActiveScene;

        public string Name;
        public Entity[] Entities;

        private Stack freedEntities;
        private int nextFreeID = 0;

        public Scene()
        {
            Entities = new Entity[MAX_ENTITIES];
            freedEntities = new Stack(50);
            Name = "Default Scene";
        }

        public Scene(string name)
        {
            Entities = new Entity[MAX_ENTITIES];
            freedEntities = new Stack(50);
            Name = name;
        }

        // Function: Adds an entity to the scene
        // ErrorHandling: Checks if entity is null or if it already exists
        // NOTE: This function should be called immediately after intialization
        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new System.Exception("entity can't be null");

            if (freedEntities.Count > 0)
                entity.ID = (int)freedEntities.Pop();
            else
                entity.ID = nextFreeID++;

            if (entity.ID >= MAX_ENTITIES)
                throw new System.Exception("Maximum number of entities is reached");

            Entities[entity.ID] = entity;
        }

        // Function: Removes an entity from the scene and destroys it!
        // ErrorHandling: Checks if entity is null or if it already exists
        public void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new System.Exception("entity can't be null");

            if (entity.ID >= MAX_ENTITIES || Entities[entity.ID] == null)
                throw new System.Exception("entity is not in the scene");

            Entities[entity.ID] = null;
            freedEntities.Push(entity.ID);
            entity.Destroy();
        }

        public Entity GetEntityWithID(int id) // O(1) complexity
        {
            if (id >= MAX_ENTITIES || Entities[id] == null)
                throw new System.Exception("entity does not exist");

            return Entities[id];
        }

        public Entity GetEntityWithName(string name) // O(n) complexity
        {
            if (string.IsNullOrEmpty(name))
                throw new System.Exception("entity name should not be empty or null");

            foreach (Entity E in Entities)
                if (E != null && E.Name.Equals(name))
                    return E;

            throw new System.Exception("entity does not exist");
        }

        // NOTE: You can use multiple content managers to manage your resources,
        // like one for levels and another for permanent resources, so you free 
        // the levels resources only
        public void Destroy(ContentManager content)
        {
            foreach (Entity E in Entities)
                if (E != null)
                    E.Destroy();

            Systems.Reset();
            Entities = null;
            System.GC.Collect();
            content.Unload();
        }

        public void Update(float deltaTime)
        {
            // Update all systems here, put them in whatever order seems fit
            Systems.Movement();
            Systems.Animation(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);

            Systems.SpriteRenderer(spriteBatch);

            spriteBatch.End();
        }
    }
}
