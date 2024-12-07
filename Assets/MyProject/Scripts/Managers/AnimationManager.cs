using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [Header("Animator Settings")]
    public Animator animator;

    private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int Motions = Animator.StringToHash("Motions");
    private static readonly int IsStrafing = Animator.StringToHash("IsStrafing"); 
    private static readonly int xInput = Animator.StringToHash("xInput");
    private static readonly int yInput = Animator.StringToHash("yInput");

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Updates animation parameters.
    /// </summary>
    /// <param name="inputMagnitude">The magnitude of the player's movement input (0 to 1).</param>
    /// <param name="isSprinting">Whether the player is sprinting or not.</param>
    /// <param name="motionState">The current motion state (0 = Start, 1 = Loop, 2 = End).</param>
    public void UpdateAnimationParameters(float inputMagnitude, bool isSprinting, float motionState, float xInput, float yInput)
    {
        animator.SetFloat(InputMagnitude, inputMagnitude);
        animator.SetBool(IsSprinting, isSprinting);
        animator.SetFloat(Motions, motionState);
        animator.SetFloat(AnimationManager.xInput, xInput);
        animator.SetFloat(AnimationManager.yInput, yInput);
    }
    
    public void UpdateStrafeState(bool isStrafing)
    {
        animator.SetBool("IsStrafing", isStrafing);
    }
}
