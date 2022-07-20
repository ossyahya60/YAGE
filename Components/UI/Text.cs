using DataOrientedEngine.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace DataOrientedEngine.Components.UI
{
    public class Text: Component
    {
        #region Variables

        public SpriteFont Font;
        public StringBuilder TEXT;
        public SpriteEffects SpriteEffects;
        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Scale;
        public Color Color;
        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation;

        #endregion

        #region Constructors

        public Text(int entityID, SpriteFont font, string text, Vector2 position)
        {
            EntityID = entityID;
            ComponentID = Engine.Components.Text;
            Font = font;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Color = Color.White;
            Rotation = 0;
            Position = position;
            TEXT = new StringBuilder(text);
            SpriteEffects = SpriteEffects.None;

            Systems.TextComps.Add(this);
        }

        public Text(int entityID, SpriteFont font, string text, Vector2 position, Vector2 origin)
        {
            EntityID = entityID;
            ComponentID = Engine.Components.Text;
            Font = font;
            Origin = origin;
            Scale = Vector2.One;
            Color = Color.White;
            Rotation = 0;
            Position = position;
            TEXT = new StringBuilder(text);
            SpriteEffects = SpriteEffects.None;

            Systems.TextComps.Add(this);
        }

        #endregion
    }
}
