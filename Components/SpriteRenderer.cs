using DataOrientedEngine.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DataOrientedEngine.Components
{
    public class SpriteRenderer: Component
    {
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public SpriteEffects SpriteEffects;
        public Vector2 Origin;
        public float Layer; //From 0 to 1 (0 is rendered last, 1 is first)
        public Color Color;
        public Vector2 Scale;
        public float Rotation;

        public SpriteRenderer(int entityID)
        {
            EntityID = entityID;
            Texture = null;
            SourceRectangle = Rectangle.Empty;
            SpriteEffects = SpriteEffects.None;
            Origin = Vector2.Zero;
            Layer = 1;
            Color = Color.White;
            Scale = Vector2.One;
            Rotation = 0;
            ComponentID = Engine.Components.SpriteRenderer;

            Systems.SpriteRendererComps.Add(this); //should be changed
        }

        public SpriteRenderer(int entityID, Texture2D texture)
        {
            EntityID = entityID;
            Texture = texture;
            SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            SpriteEffects = SpriteEffects.None;
            Origin = Vector2.Zero;
            Layer = 1;
            Color = Color.White;
            Scale = Vector2.One;
            Rotation = 0;
            ComponentID = Engine.Components.SpriteRenderer;

            Systems.SpriteRendererComps.Add(this); //should be changed
        }
    }
}
