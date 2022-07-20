using DataOrientedEngine.Engine;

namespace DataOrientedEngine.Components
{
    public class Movement: Component
    {
        public float XPosition;
        public float YPosition;
        public float DeltaX;
        public float DeltaY;

        public Movement(int entityID)
        {
            XPosition = 0;
            YPosition = 0;
            DeltaX = 0;
            DeltaY = 0;
            EntityID = entityID;
            ComponentID = Engine.Components.Movement;

            Systems.MovementComps.Insert(entityID, this); //should be changed
        }

        public Movement(int entityID, float posX, float posY)
        {
            XPosition = posX;
            YPosition = posY;
            DeltaX = 0;
            DeltaY = 0;
            EntityID = entityID;
            ComponentID = Engine.Components.Movement;

            Systems.MovementComps.Insert(entityID, this); //should be changed
        }
    }
}
