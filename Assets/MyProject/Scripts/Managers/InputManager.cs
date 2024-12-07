using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public PlayerControls playerControls;
        
        public Vector2 movementInput;
        public Vector2 cameraInput;

        public float cameraInputX;
        public float cameraInputY;
        public float moveAmount;
        
        public float horizontalInput;
        public float verticalInput;
        public bool isSprinting;

        public bool lightAttackInput;
        public bool heavyAttackInput;
        
        
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                
                playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.Player.Move.canceled += i => movementInput = Vector2.zero;
                
                playerControls.Player.Sprint.performed += _ => isSprinting = true;
                playerControls.Player.Sprint.canceled += _ => isSprinting = false;

                playerControls.Player.LightAttack.performed += _ => lightAttackInput = true;
                playerControls.Player.LightAttack.canceled += _ => lightAttackInput = false;
                
                playerControls.Player.HeavyAttack.performed += _ => heavyAttackInput = true;
                playerControls.Player.HeavyAttack.canceled += _ => heavyAttackInput = false;
                
                playerControls.Player.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.Player.Camera.canceled += i => cameraInput = Vector2.zero;
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
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
            
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }

        private void HandleSprintInput()
        {
            
        }
    }
}
