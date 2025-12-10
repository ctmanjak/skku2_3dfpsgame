using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInput : MonoBehaviour
    {
        private bool _wantsToSprint;
        public bool WantsToSprint => _wantsToSprint;

        private bool _wantsToJump;
        public bool WantsToJump => _wantsToJump;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) _wantsToSprint = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) _wantsToSprint = false;
            
            if (Input.GetButtonDown("Jump")) _wantsToJump = true;
        }

        public void Grounding()
        {
            _wantsToJump = false;
        }
    }
}