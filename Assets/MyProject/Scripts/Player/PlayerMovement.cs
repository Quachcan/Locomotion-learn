using UnityEngine;

namespace MyProject.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float sprintMultiplier = 2;
        public float rotationSpeed = 10f;

        [Header("References")]
        public AnimationManager animationManager;

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
            if (animationManager == null)
            {
                animationManager = GetComponent<AnimationManager>();
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

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleInput()
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
        
            xInput = inputHorizontal;
            yInput = inputVertical;

            isSprinting = Input.GetKey(KeyCode.LeftShift);
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
                Debug.Log($"IsStrafing set to: {isStrafing}");
                
                Invoke(nameof(ResetToggleCoolDown), 0.2f);
            }
        }

        private void ResetToggleCoolDown()
        {
            toggleCoolDown = false;
        }

        private void HandleMovement()
        {
            // Calculate direction based on input
            moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;

            // Adjust speed for sprinting
            float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

            // Apply movement if the player is moving
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
            float targetMagnitude = Mathf.Clamp01(new Vector2(inputHorizontal, inputVertical).magnitude);

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
            float inputMagnitude = Mathf.Clamp01(new Vector2(inputHorizontal, inputVertical).magnitude);

            // Determine motion state (0 = Start, 1 = Loop, 2 = End)
            float motionState = isMoving ? 1 : 2; // 1 = Loop when moving, 2 = End when stopping
            if (!isMoving && rb.linearVelocity.magnitude > 0.1f) motionState = 0; // Play Start animation when starting

            // Send parameters to AnimationManager
            animationManager.UpdateAnimationParameters(smoothedInputMagnitude, isSprinting, motionState, xInput, yInput);
        }
    }
}