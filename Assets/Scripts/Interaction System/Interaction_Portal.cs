using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace HA
{
    public class Interaction_Portal : MonoBehaviour, IInteractable
    {
        public GameObject bossZone;
        public GameObject bossMonster;
        public Canvas bossHPBar;

        public string Key => "Interaction_Portal";

        public string Message => "Warp To Ground";

        public void Start()
        {
            bossHPBar.planeDistance = -0.5f;
            bossMonster.SetActive(false);
        }

        public void Interact()
        {
            Debug.Log("워프 이동");

            var player = GameManager.Instance.currentActiveCharacter;
            var controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false; // 일시적으로 CharacterController 비활성화
                player.transform.position = bossZone.transform.position;
                controller.enabled = true; // 다시 활성화 (물리 충돌 정상 적용)
            }

            bossHPBar.planeDistance = 1.3f;
            bossMonster.SetActive(true);
            //GameManager.Instance.characters[0].transform.position = bossZone.transform.position;
            //GameManager.Instance.characters[1].transform.position = bossZone.transform.position;
        }

        
    }
}
