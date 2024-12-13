using System;
using MyProject.Scripts.ScriptableObjects.Weapons;
using UnityEngine;

namespace MyProject.Scripts.Weapon
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Weapon_SO weaponSo;

        [SerializeField] private Collider weaponCollider;

        private bool isAttacking;
        private void Start()
        {
            weaponCollider.enabled = false;
            
            weaponCollider.transform.localPosition = weaponSo.colliderOffset;
            weaponCollider.transform.localScale = weaponSo.colliderSize;
        }

        public void OnAttack()
        {
            weaponCollider.enabled = true;
            isAttacking = true;
        }

        public void OffAttack()
        {
            weaponCollider.enabled = false;
            isAttacking = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isAttacking) return;

            if (other.CompareTag("Enemy"))
            {
                var enemyHealth = other.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    float damage = weaponSo.baseDamage;
                    enemyHealth.TakeDamage(damage);
                    Debug.Log("Dealt " + damage + " damage to " + other.name);
                }
                else
                {
                    Debug.Log("Enemy does not have Health");
                }
            }
        }

        public Weapon_SO WeaponData()
        {
            return weaponSo;
        }
    }
}
