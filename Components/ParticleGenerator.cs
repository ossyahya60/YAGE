using DataOrientedEngine.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DataOrientedEngine.Components
{
    public class ParticleGenerator: Component
    {
        public const int MAX_PARTICLES = 5000;

        public Particle[] Particles;
        public bool RandomDirection;
        public Color SpawnColor;
        public Color FadeColor;
        public Vector2 Direction;
        public Texture2D renderTarget;

        public ParticleGenerator(int entityID, GraphicsDevice graphics)
        {
            Particles = new Particle[MAX_PARTICLES];
            RandomDirection = true;
            SpawnColor = Color.White;
            FadeColor = Color.Black;
            Direction = Vector2.UnitY;
            EntityID = entityID;
            ComponentID = Engine.Components.ParticleGenerator;
            renderTarget = new Texture2D(graphics, graphics.Viewport.Width, graphics.Viewport.Height);

            System.Random random = new System.Random();
            Rectangle viewport = graphics.Viewport.Bounds;
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                Particles[i] = new Particle();
                Particles[i].Position.X = random.Next(viewport.X, viewport.Width - 1);
                Particles[i].Position.Y = random.Next(viewport.Y, viewport.Height - 1);
                Particles[i].Velocity.Y = random.Next(50, 200);
            }

            Systems.ParticleComps[Systems.AddComponent(Scene.ActiveScene.GetEntityWithID(entityID), this)] = this;
        }
    }
}

