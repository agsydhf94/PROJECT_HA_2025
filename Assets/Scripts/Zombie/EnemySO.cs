using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(menuName = "ScriptableObject/EnemySO", fileName = "EnemySO")]
    public class EnemySO : ScriptableObject
    {
        public int enemyID;
        public float health = 100f;
        public float damage = 20f;
        public float speed = 1.8f;
        public float attackSpeed;
        public float expGranted;
        public Color skinColor = Color.white;
        public EnemyType enemyType;


    }

    public enum EnemyType
    {
        Shooter,
        Zombie
    }
}
