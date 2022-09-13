using Microsoft.Xna.Framework;

namespace DataOrientedEngine.Components
{
    public class Particle
    {
        public float Lifetime;
        public bool Died;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public int SizeX;
        public int SizeY;
        public Color Color;

        public Particle()
        {
            Lifetime = 1;
            Died = false;
            Position = Vector2.Zero;
            Velocity = Vector2.One * 10;
            Rotation = 0;
            SizeX = 1;
            SizeY = 3;
            Color = Color.White;
        }
    }
}