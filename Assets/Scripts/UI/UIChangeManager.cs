using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class UIChangeManager : MonoBehaviour
    {
        PlayerControllerRPG playerControllerRPG = PlayerControllerRPG.Instance;

        private void Update()
        {
            gameObject.SetActive(!playerControllerRPG.isRPG);
        }
    }
}