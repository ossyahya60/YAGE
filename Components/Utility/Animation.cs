using Microsoft.Xna.Framework;

namespace DataOrientedEngine.Components.Utility
{
    public class Animation
    {
        public string Name;
        public float FrameDelay;
        public bool Reversed;
        public bool IsPlaying;
        public bool Loop;
        public int CurrentFrame;
        public float Accumulator;
        public float Speed;
        public Rectangle[] AnimationFrames;

        public Animation(string name, Rectangle[] animationFrames)
        {
            Name = name;
            CurrentFrame = 0;
            FrameDelay = 0.5f;
            Reversed = false;
            Loop = false;
            IsPlaying = false;
            Accumulator = 0;
            Speed = 1;
            AnimationFrames = animationFrames;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            IsPlaying = true;
        }
    }
}
