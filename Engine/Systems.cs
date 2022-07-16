using System.Collections.Generic;
using DataOrientedEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DataOrientedEngine.Engine
{
    public static class Systems
    {
        private static readonly int initialCapacity = 50;

        public static List<Movement> MovementComps;
        public static List<SpriteRenderer> SpriteRendererComps;
        public static List<Animator> AnimatorComps;

        static Systems()
        {
            MovementComps = new List<Movement>(initialCapacity);
            SpriteRendererComps = new List<SpriteRenderer>(initialCapacity);
            AnimatorComps = new List<Animator>(initialCapacity);
        }

        // Movement System
        // This system is responsible for moving any entity in the scene
        public static void Movement()
        {
            foreach (Movement m in MovementComps)
            {
                Entity entity = Scene.ActiveScene.GetEntityWithID(m.EntityID);

                if (entity.Active && m.Enabled)
                {
                    m.XPosition += m.DeltaX;
                    m.YPosition += m.DeltaY;

                    m.DeltaX = 0;
                    m.DeltaY = 0;
                }
            }
        }

        // This system is responsible for rendering sprites on the screen
        public static void SpriteRenderer(SpriteBatch spriteBatch)
        {
            foreach (SpriteRenderer sr in SpriteRendererComps)
            {
                Entity entity = Scene.ActiveScene.GetEntityWithID(sr.EntityID);

                if (entity.Active && sr.Enabled && entity.HasComponent(Components.Movement))
                {
                    Movement movement = MovementComps[entity.ID];

                    spriteBatch.Draw(sr.Texture, new Vector2(movement.XPosition, movement.YPosition), sr.SourceRectangle, sr.Color, sr.Rotation, sr.Origin, sr.Scale, sr.SpriteEffects, sr.Layer);
                }
            }
        }

        // This system is responsible for handling animations of an entity
        public static void Animation(float deltaTime)
        {
            foreach (Animator A in AnimatorComps)
            {
                Entity entity = Scene.ActiveScene.GetEntityWithID(A.EntityID);

                if (entity.Active && A.Enabled)
                {
                    if (A.IsSomethingPlaying())
                    {
                        SpriteRenderer spriteRenderer = SpriteRendererComps[entity.ID];
                        DataOrientedEngine.Components.Utility.Animation animation = A.currentAnimation;
                        spriteRenderer.SourceRectangle = animation.AnimationFrames[animation.CurrentFrame];

                        if (animation.Accumulator >= animation.FrameDelay)
                        {
                            bool finishCondition = (!animation.Reversed) ? animation.CurrentFrame + 1 >= animation.AnimationFrames.Length : false;
                            finishCondition = (animation.Reversed) ? animation.CurrentFrame <= 0 : finishCondition;

                            if (finishCondition)
                            {
                                if (animation.Loop)
                                    A.Play(animation.Name);
                                else
                                    A.currentAnimation = null;
                            }
                            else
                            {
                                animation.Accumulator = 0;
                                animation.CurrentFrame = (!animation.Reversed) ? animation.CurrentFrame + 1 : animation.CurrentFrame - 1;
                            }
                        }
                        else
                        {
                            animation.Accumulator += deltaTime * animation.Speed;
                        }
                    }
                }
            }
        }
    }
}
