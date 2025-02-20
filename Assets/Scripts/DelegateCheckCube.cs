using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HA.QuestManager;

namespace HA
{
    public class DelegateCheckCube : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            QuestManager.Instance.CheckSubscribedMethods();
        }
    }
}
