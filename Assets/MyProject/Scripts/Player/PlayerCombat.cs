using MyProject.Scripts.Managers;
using MyProject.Scripts.Weapon;
using UnityEngine;

namespace MyProject.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private WeaponHandler weaponHandler;
        
        [Header("Combo Data")]
        [SerializeField] 
        private ComboData comboData;
        
        [Header("Setting")]
        [SerializeField]
        private Animator animator; 
        [SerializeField]
        private InputManager inputManager;
        
        [Header("Combo Settings")]
        [SerializeField]
        private int currentComboStep;
        [SerializeField]
        private float comboResetTime = 3f; 
        [SerializeField]
        private float lastInputTime;
        [SerializeField]
        private bool isComboFinished;
        //private float minComboInputTime = 1f; 
        [SerializeField]
        private bool canProceedNextStep = false;
        [SerializeField]
        private bool attackActive = false;
        public bool isAttacking = false;
        

        private void Start()
        {
            canProceedNextStep = true;
            animator = GetComponent<Animator>();
            inputManager = GetComponent<InputManager>();
        }
        private void Update()
        {
            bool lightPressed = inputManager.lightAttackInput;
            bool heavyPressed = inputManager.heavyAttackInput;

            if ((lightPressed || heavyPressed) && canProceedNextStep)
            {
                AttackType inputType = lightPressed ? AttackType.LightAttack : AttackType.HeavyAttack;
                HandleComboInput(inputType);
            }

            if (Time.time - lastInputTime > comboResetTime && currentComboStep > 0 && currentComboStep < comboData.comboInputs.Length)
            {
                ResetCombo();
            }
        }

        private void HandleComboInput(AttackType inputType)
        {
            if (comboData == null || comboData.comboClips == null || comboData.comboClips.Length == 0) return;

            // Nếu vượt quá số bước combo, nghĩa là combo đã xong, đợi OnComboEnd()
            if (currentComboStep >= comboData.comboInputs.Length)
            {
                return;
            }

            if (comboData.comboInputs[currentComboStep] == inputType)
            {
                PlayComboClip(currentComboStep);
                currentComboStep++;
                lastInputTime = Time.time;
                
                canProceedNextStep = false;
            }
            else
            {
                ResetCombo();
            }
        }

        private void PlayComboClip(int step)
        {
            if (step < comboData.comboClips.Length)
            {
                var clip = comboData.comboClips[step].comboClip;
                if (clip != null)
                {
                    animator.Play(clip.name, 0, 0f);
                }
            }
        }

        private void ResetCombo()
        {
            currentComboStep = 0;
            lastInputTime = Time.time;
            canProceedNextStep = true;
            attackActive = false;
        }

        public void OnComboBegin()
        {
            isComboFinished = true;
            canProceedNextStep = false;
            Debug.Log("OnComboBegin");
        }

        public void OnFreeFLow()
        {
            canProceedNextStep = true;
            Debug.Log("OnFreeFLow");
        }
        
        public void OnAttack()
        {
            isAttacking = true;
            attackActive = true;
            weaponHandler.OnAttack();
            Debug.Log("OnAttack");
        }

        public void OffAttack()
        {
            attackActive = false;
            Debug.Log("OffAttack");
        }

        public void OnComboEnd()
        {
            isAttacking = false;
            weaponHandler.OffAttack();
            ResetCombo();
            Debug.Log("OnComboEnd");
        }
        
    }
}