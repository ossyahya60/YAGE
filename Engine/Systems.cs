﻿using System.Collections.Generic;
using DataOrientedEngine.Components;
using DataOrientedEngine.Components.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DataOrientedEngine.Engine
{
    public static class Systems
    {
        public static Movement[] MovementComps;
        public static SpriteRenderer[] SpriteRendererComps;
        public static Animator[] AnimatorComps;
        public static ParticleGenerator[] ParticleComps;
        public static Text[] TextComps;

        private static int[] lastAllocatedIndex;
        private static Stack<int>[] freedComponents;

        static Systems()
        {
            Init();
        }

        private static void Init()
        {
            MovementComps = new Movement[Scene.MAX_ENTITIES];
            SpriteRendererComps = new SpriteRenderer[Scene.MAX_ENTITIES];
            AnimatorComps = new Animator[Scene.MAX_ENTITIES];
            ParticleComps = new ParticleGenerator[Scene.MAX_ENTITIES];
            TextComps = new Text[Scene.MAX_ENTITIES];
            lastAllocatedIndex = new int[System.Enum.GetValues(typeof(Components)).Length];
            freedComponents = new Stack<int>[System.Enum.GetValues(typeof(Components)).Length];

            for (int i = 0; i < freedComponents.Length; i++)
                freedComponents[i] = new Stack<int>();
        }

        public static void Reset()
        {
            Init();
        }

        /// <summary>
        /// returns the index of the component that should be added to the respective array system
        /// </summary>
        /// <param name="comps"></param>
        /// <param name="entity"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static int AddComponent(Entity entity, Component comp)
        {
            int compType = (int)comp.ComponentID;

            if (entity.HasComponent(comp.ComponentID))
                throw new System.Exception("Entity already has that component");

            if (freedComponents[compType].Count > 0)
            {
                entity.componentIndex[compType] = freedComponents[compType].Pop();
                return entity.componentIndex[compType];
            }
            else
            {
                entity.componentIndex[compType] = lastAllocatedIndex[compType]++;
                return entity.componentIndex[compType];
            }

        }

        /// <summary>
        /// returns the index of the component from an entity if exists, you should remove it from the components array
        /// </summary>
        /// <param name="comps"></param>
        /// <param name="entity"></param>
        /// <param name="componentType"></param>
        /// <exception cref="System.Exception"></exception>
        public static void RemoveComponent(Entity entity, Components componentType)
        {
            if (!entity.HasComponent(componentType))
                throw new System.Exception("Entity does not have that component");

            freedComponents[(int)componentType].Push(entity.componentIndex[(int)componentType]);
            entity.componentIndex[(int)componentType] = -1;
        }

        // Movement System
        // This system is responsible for moving any entity in the scene
        public static void Movement()
        {
            for (int i = 0; i < lastAllocatedIndex[(int)Components.Movement]; i++)
            {
                Movement m = MovementComps[i];
                if (m == null)
                    continue;

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
            for (int i = 0; i < lastAllocatedIndex[(int)Components.SpriteRenderer]; i++)
            {
                SpriteRenderer sr = SpriteRendererComps[i];
                if (sr == null)
                    continue;

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
            for (int i = 0; i < lastAllocatedIndex[(int)Components.Animator]; i++)
            {
                Animator A = AnimatorComps[i];
                if (A == null)
                    continue;

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

        //TODO: Needs Rework
        // This system is responsible for generating particles in space
        public static void ParticleGenerationUpdate(float deltatime)
        {
            for (int i = 0; i < lastAllocatedIndex[(int)Components.ParticleGenerator]; i++)
            {
                ParticleGenerator PG = ParticleComps[i];
                if (PG == null)
                    continue;

                Entity entity = Scene.ActiveScene.GetEntityWithID(PG.EntityID);

                if (entity.Active && PG.Enabled)
                {
                    Movement movement = MovementComps[entity.ID];

                    Vector2 referencePosition = new Vector2(movement.XPosition, movement.YPosition);
                    Color[] pixels = new Color[PG.renderTarget.Width * PG.renderTarget.Height];

                    for (int j = 0; j < ParticleGenerator.MAX_PARTICLES; j++)
                    {
                        Particle temp = PG.Particles[j];
                        temp.Position += temp.Velocity * deltatime;

                        if (temp.Position.X < -temp.SizeX + 1 || temp.Position.X > PG.renderTarget.Width - 1 || temp.Position.Y < -temp.SizeY + 1 || temp.Position.Y > PG.renderTarget.Height - 1)
                            continue;

                        /*
                        for (int sx = 0; sx < temp.SizeX; sx++)
                            pixels[(int)(temp.Position.Y * PG.renderTarget.Width + temp.Position.X + sx)] = temp.Color;
                        for (int sy = 1; sy < temp.SizeY; sy++)
                            pixels[(int)((temp.Position.Y + sy) * PG.renderTarget.Width + temp.Position.X)] = temp.Color;
                        */
                    }

                    PG.renderTarget.SetData(pixels);
                }
            }
        }

        // This system is responsible for rendering particles in space
        public static void ParticleGenerationDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < lastAllocatedIndex[(int)Components.ParticleGenerator]; i++)
            {
                ParticleGenerator PG = ParticleComps[i];
                if (PG == null)
                    continue;

                Entity entity = Scene.ActiveScene.GetEntityWithID(PG.EntityID);

                if (entity.Active && PG.Enabled)
                {
                    spriteBatch.Draw(PG.renderTarget, PG.renderTarget.GraphicsDevice.Viewport.Bounds, PG.SpawnColor);
                }
            }
        }

        // This system renders text to screen space
        public static void TextRendering(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < lastAllocatedIndex[(int)Components.Text]; i++)
            {
                Text txt = TextComps[i];
                if (txt == null)
                    continue;

                Entity entity = Scene.ActiveScene.GetEntityWithID(txt.EntityID);

                if (entity.Active && txt.Enabled)
                {
                    spriteBatch.DrawString(txt.Font, txt.TEXT, txt.Position, txt.Color, txt.Rotation, txt.Origin, txt.Scale, txt.SpriteEffects, 0);
                }
            }
        }
    }
}