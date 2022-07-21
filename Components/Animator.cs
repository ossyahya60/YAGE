using DataOrientedEngine.Engine;
using DataOrientedEngine.Components.Utility;
using System.Collections.Generic;

namespace DataOrientedEngine.Components
{
    public class Animator: Component
    {
        public Dictionary<string, Animation> Animations;
        public Animation currentAnimation;

        public Animator(int entityID)
        {
            Animations = new Dictionary<string, Animation>();
            currentAnimation = null;

            EntityID = entityID;
            ComponentID = Engine.Components.Animator;

            Systems.AnimatorComps[Systems.AddComponent(Scene.ActiveScene.GetEntityWithID(entityID), this)] = this;
        }

        public bool IsSomethingPlaying()
        {
            return currentAnimation != null? currentAnimation.IsPlaying : false;
        }

        /// <summary>
        /// This functions plays a brand new animation and cancels the current one
        /// </summary>
        /// <param name="animationName"></param>
        public void Play(string animationName)
        {
            Animation animation = null;
            if (Animations.TryGetValue(animationName, out animation))
            {
                currentAnimation = animation;
                animation.Accumulator = 0;
                animation.CurrentFrame = !animation.Reversed? 0 : animation.AnimationFrames.Length - 1;
                animation.IsPlaying = true;
            }
        }

        public void Resume()
        {
            if (currentAnimation != null)
                currentAnimation.IsPlaying = true;
        }

        public void Stop()
        {
            if (currentAnimation != null)
                currentAnimation.IsPlaying = false;
        }
    }
}
