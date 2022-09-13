namespace DataOrientedEngine.Engine
{
    // This class is the base class of all 
    // components, it contains the common data 
    // between all components

    // All component names must be here!
    public enum Components
    {
        Movement,
        SpriteRenderer,
        Animator,
        ParticleGenerator,
        // UI
        Text
    }

    public class Component
    {
        public int EntityID;
        public bool Enabled = true;

        internal Components ComponentID; // This represents the fixed id of a component like (Transform: 0, Rigidbody: 1)
    }
}
