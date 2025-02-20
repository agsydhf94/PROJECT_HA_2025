using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace HA
{
    [System.Serializable]
    [CreateAssetMenu]
    public class SkillSO : ScriptableObject
    {
        public ISkillState skillState;
        public string skillName;
        public string skillDescription;

        public float attackPoint;
        public float attackRange;
        public float skillHalfAngle;
        public LayerMask enemyLayer;

        public Sprite skillSprite;

    }
}
