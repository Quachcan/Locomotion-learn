using UnityEngine;

public enum AttackType
{
    LightAttack,
    HeavyAttack
}

public enum ArmedType
{
    Main,
    Second
}

public enum TraceType
{
    Sphere,
    Capsule,
    Box
}

public enum AttackDirection
{
    Front,
    Back,
    Right,
    Left,
    Up,
    Down
}

[System.Serializable]
public class HitTraceData
{
    // Defines how we detect hits (e.g., using a sphere, capsule, or box around certain bones).
    public ArmedType armedType = ArmedType.Main;
    public TraceType traceType = TraceType.Sphere;
    public Transform[] bones;
    public float traceRadius = 0.5f;
    public float traceAngle = 60f;
    public float traceHeight = 1f;
}

[System.Serializable]
public class HitAnimationData
{
    // Defines which animations to play on the target when they are hit, block, parry, etc.
    public AnimationClip hitClip;
    public AnimationClip blockHitClip;
    public AnimationClip parryingClip;
    public AnimationClip parryingReactionClip;
    public float transitionDuration = 0.2f;
    public bool useParryingReaction = false;
}

[System.Serializable]
public class HitKnockBackData
{
    // Defines the knockback behavior on the target.
    public bool isKnockback = false;
    public float knockBackForce = 5f;
    public float knockBackTime = 0.5f;
    public float intensity = 1f;
    public bool wallHitable = false;
}

[System.Serializable]
public class AttackParticleFxData
{
    public float damage;
    public bool isBlockAble;
    public bool isParryAble;
}

[System.Serializable]
public class HitReactionData
{
    // Each hit reaction entry defines what happens when the target is hit.
    // It includes trace data, what type of attack hit them, from which direction,
    // and what animations and knockback effects to apply.
    
    public HitTraceData hitTraceData;

    public AttackType attackType;
    public AttackDirection attackDirection;

    public HitAnimationData hitAnimationData;
    public HitKnockBackData hitKnockBackData;

    public AttackParticleFxData attackParticleFxData;
}

[System.Serializable]
public class ComboClipData
{
    // Each combo clip represents one attack step in the combo.
    // If the combo has multiple steps, you’ll have multiple ComboClipData entries.
    // Each step can have multiple HitReactionData entries defining different hit scenarios.
    public AnimationClip comboClip;
    public HitReactionData[] hitReactionDatas;
}

[CreateAssetMenu(fileName = "NewComboData", menuName = "Combat/Combo Data")]
public class ComboData : ScriptableObject
{
    public string comboName;
    
    // Sequence of inputs (e.g., [LightAttack, LightAttack, HeavyAttack]) that forms the combo.
    // Each input corresponds to one combo clip.
    public AttackType[] comboInputs;
    
    // Each element corresponds to a step in the combo.
    // If comboInputs = [LightAttack, LightAttack], then comboClips should also have 2 elements.
    // Each ComboClipData defines the animation and hit reactions for that step of the combo.
    public ComboClipData[] comboClips;
}