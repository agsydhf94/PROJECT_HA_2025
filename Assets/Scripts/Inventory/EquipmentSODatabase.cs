using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HA
{
    public class EquipmentSODatabase : MonoBehaviour
    {
        public static EquipmentSODatabase Instance { get; private set; }

        //public EquipmentSO[] equipmentSOs;
        public List<EquipmentSO> equipmentSOs = new List<EquipmentSO>();


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
