using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public PlayerControls playerControls;
        
        public Vector2 movementInput;
        public float horizontalInput;
        public float verticalInput;
        public bool isSprinting;
        
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                
                playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.Player.Move.canceled += i => movementInput = Vector2.zero;
                
                playerControls.Player.Sprint.performed += _ => isSprinting = true;
                playerControls.Player.Sprint.canceled += _ => isSprinting = false;
            }
            
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
        
        private void Update()
        {
        
        }

        public void HandleAllInputs()
        {
            HandleMovementInput();
            //HandleJumpInput();
            //HandleActionInput();
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
        }

        private void HandleSprintInput()
        {
            
        }
    }
}
