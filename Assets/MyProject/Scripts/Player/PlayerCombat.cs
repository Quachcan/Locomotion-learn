using UnityEngine;

namespace MyProject.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combo Data")]
        [SerializeField] private ComboData comboData;

        [Header("Setting")]
        [SerializeField] private Animator animator; 

        private int currentComboStep = 0;
        private float comboResetTime = 3f; 
        private float lastInputTime = 0f;
        private bool isComboFinished;
        
        private float minComboInputTime = 1f; 
        
        private bool canProceedNextStep = false;
        private bool attackActive = false;


        private void Start()
        {
            canProceedNextStep = true;
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            bool lightPressed = Input.GetKeyDown(KeyCode.Mouse0);
            bool heavyPressed = Input.GetKeyDown(KeyCode.Mouse1);

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

                // Tạm thời khóa input combo tiếp theo, chờ OnFreeFlow trong animation clip này gọi
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
            canProceedNextStep = false;
        }

        public void OnFreeFLow()
        {
            canProceedNextStep = true;
        }

        public void OnAttack()
        {
            attackActive = true;
            Debug.Log("OnAttack");
        }

        public void OffAttack()
        {
            attackActive = false;
        }

        public void OnComboEnd()
        {
            ResetCombo();
        }
    }
}