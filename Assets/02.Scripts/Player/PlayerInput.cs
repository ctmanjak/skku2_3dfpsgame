using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveAxis { get; private set; }
        
        public bool SprintHeld  { get; private set; }
        
        public bool JumpPressed { get; private set; }
        public bool BombPressed { get; private set; }

        void Update()
        {
            MoveAxis = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            
            SprintHeld = Input.GetKey(KeyCode.LeftShift);
            
            JumpPressed = Input.GetButtonDown("Jump");
            BombPressed = Input.GetKeyDown(KeyCode.G);
        }

        public bool ConsumeJump()
        {
            if (!JumpPressed) return false;
            JumpPressed = false;
            return true;
        }

        public bool ConsumeBomb()
        {
            if (!BombPressed) return false;
            BombPressed = false;
            return true;
        }
    }
}