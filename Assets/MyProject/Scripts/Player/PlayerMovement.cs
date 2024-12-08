using MyProject.Scripts.Managers;
using UnityEngine;

namespace MyProject.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float rotationSpeed = 10f;
        public float walkSpeed = 2.5f;
        public float runSpeed = 5f;
        public float sprintMultiplier = 2;

        [Header("References")]
        public AnimationManager animationManager;
        public InputManager inputManager;
        private Transform cameraTransform;
        private PlayerCombat playerCombat;

        private Vector3 moveDirection;
        private Rigidbody rb;

        private float inputHorizontal;
        private float inputVertical;
    
        private float xInput;
        private float yInput;
    
        public bool isStrafing = false;
        private bool isSprinting;
        private bool isMoving;
        private bool toggleCoolDown;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            cameraTransform = Camera.main?.transform;
            playerCombat = GetComponent<PlayerCombat>();
            if (animationManager == null)
            {
                animationManager = GetComponent<AnimationManager>();
            }

            if (inputManager == null)
            {
                inputManager = GetComponent<InputManager>();
            }
        }

        void Update()
        {
            HandleInput();
            HandleRotation();
            UpdateInputMagnitude();
            SendMovementToAnimator();
            HandleStrafeToggle();
        }

        public void HandleAllMovement()
        {
            HandleMovement();
        }

        private void HandleInput()
        {
            //inputHorizontal = Input.GetAxis("Horizontal");
            //inputVertical = Input.GetAxis("Vertical");
        
            xInput = inputManager.horizontalInput;
            yInput = inputManager.verticalInput;

            isSprinting = inputManager.isSprinting;
            // Check if player is moving
            isMoving = Mathf.Abs(xInput) > 0.1f || Mathf.Abs(yInput) > 0.1f;
        }

        private void HandleStrafeToggle()
        {
            if (Input.GetKey(KeyCode.Tab) && !toggleCoolDown)
            {
                toggleCoolDown = true; 
                isStrafing = !isStrafing;
                animationManager.UpdateStrafeState(isStrafing);
                
                Invoke(nameof(ResetToggleCoolDown), 0.2f);
            }
        }

        private void ResetToggleCoolDown()
        {
            toggleCoolDown = false;
        }

        private void HandleMovement()
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            
            forward.y = 0;
            right.y = 0;
            
            forward.Normalize();
            right.Normalize();
            
            moveDirection = (forward * inputManager.verticalInput + right * inputManager.horizontalInput).normalized;

            // Adjust speed for sprinting
            float currentSpeed = walkSpeed;

            if (isMoving && inputManager.isSprinting)
            {
                currentSpeed = runSpeed * sprintMultiplier;
            }
            else if (isMoving && inputManager.moveAmount > 0.5f)
            {
                currentSpeed = runSpeed;
            }
            
            if (isMoving)
            {
                Vector3 velocity = moveDirection * currentSpeed;
                rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }

        private void HandleRotation()
        {
            if (isMoving && isStrafing)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else if (isMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private float smoothedInputMagnitude; 

        private void UpdateInputMagnitude()
        {
            // Calculate InputMagnitude target value 
            float targetMagnitude = Mathf.Clamp01(new Vector2(inputManager.horizontalInput, inputManager.verticalInput).magnitude);

            if (isSprinting && isMoving)
            {
                targetMagnitude = 1.5f;
            }

            // Làm mượt giá trị InputMagnitude bằng Lerp
            smoothedInputMagnitude = Mathf.Lerp(smoothedInputMagnitude, targetMagnitude, Time.deltaTime * 5f); // 5f là tốc độ mượt
        }

        private void SendMovementToAnimator()
        {
            // Calculate input magnitude for Blend Tree
            float inputMagnitude = Mathf.Clamp01(new Vector2(inputManager.horizontalInput, inputManager.verticalInput).magnitude);

            // Determine motion state (0 = Start, 1 = Loop, 2 = End)
            float motionState = isMoving ? 1 : 2; // 1 = Loop when moving, 2 = End when stopping
            if (!isMoving && rb.linearVelocity.magnitude > 0.1f) motionState = 0; // Play Start animation when starting

            // Send parameters to AnimationManager
            animationManager.UpdateAnimationParameters(smoothedInputMagnitude, isSprinting, motionState, xInput, yInput);
        }
    }
}