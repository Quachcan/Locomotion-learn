using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = System.Numerics.Vector2;

namespace MyProject.Scripts.ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
    public class Weapon_SO : ScriptableObject
    {
        [Header("Basic Settings")]
        public string weaponName;
        public Sprite weaponIcon;
        
        [Header("Damage Settings")]
        public float baseDamage;
        public float criticalDamageMultiplier;

        [Header("Attack Settings")] 
        public float attackRange;
        
        [Header("Collision Settings")]
        public Vector3 colliderOffset = Vector3.zero;

        public Vector3 colliderSize = new Vector3(1f, 1f, 1f);
    }
}

public enum WeaponType
{
    Sword,
    ColossalSword,
    Bow
}
