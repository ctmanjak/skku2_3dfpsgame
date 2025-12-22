using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveAxis { get; private set; }
        public Vector2 AimAxis { get; private set; }
        
        public bool SprintHeld  { get; private set; }
        public bool FireHeld  { get; private set; }
        
        public bool JumpPressed { get; private set; }
        public bool BombPressed { get; private set; }
        public bool ReloadPressed { get; private set; }
        public bool ChangeViewPressed { get; private set; }
        public bool UnlockCursorPressed { get; private set; }
        public bool ZoomInPressed { get; private set; }

        void Update()
        {
            MoveAxis = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            AimAxis = new Vector2(
                Input.GetAxis("Mouse X"),
                -Input.GetAxis("Mouse Y")
            );
            
            SprintHeld = Input.GetKey(KeyCode.LeftShift);
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                FireHeld = Input.GetMouseButton(0);
            }
            
            JumpPressed = Input.GetButtonDown("Jump");
            BombPressed = Input.GetKeyDown(KeyCode.G);
            ReloadPressed = Input.GetKeyDown(KeyCode.R);
            ChangeViewPressed = Input.GetKeyDown(KeyCode.T);
            UnlockCursorPressed = Input.GetKeyDown(KeyCode.Escape);
            ZoomInPressed = Input.GetKeyDown(KeyCode.X);
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

        public bool ConsumeReload()
        {
            if (!ReloadPressed) return false;
            ReloadPressed = false;
            return true;
        }

        public bool ConsumeChangeView()
        {
            if (!ChangeViewPressed) return false;
            ChangeViewPressed = false;
            return true;
        }

        public bool ConsumeUnlockCursor()
        {
            if (!UnlockCursorPressed) return false;
            UnlockCursorPressed = false;
            return true;
        }

        public bool ConsumeZoomIn()
        {
            if (!ZoomInPressed) return false;
            ZoomInPressed = false;
            return true;
        }
    }
}