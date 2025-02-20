using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class UserDataDTO : MonoBehaviour
    {
        
    }

    [System.Serializable]
    public class PlayerDataDTO : UserDataDTO
    {
        public string playerName;
        public int level;

        public float asuna_CurrentHealth;
        public float asuna_MaxHealth;
        public SkillSlot[] asuna_Skill;
        public Vector3 asuna_Position;

        public float kunoichi_CurrentHealth;
        public float kunoichi_MaxHealth;
        public SkillSlot[] kunoichi_Skill;
        public Vector3 kunoichi_Position;

    }
}
