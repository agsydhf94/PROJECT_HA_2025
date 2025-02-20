using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Interaction_Shop : MonoBehaviour, IInteractable
    {
        public string Key => "Shop";

        public string Message => "Shop";

        

        public void Interact()
        {
            ShopUI.Instance.currentMoneyText.text = GameManager.Instance.money.ToString();

            ShopUI.Instance.shopUICanvas.planeDistance = 1.3f;
            InteractionUI.Instance.gameObject.SetActive(false);

            ShopUI.Instance._npcCamera.Priority = 20;
            ShopUI.Instance._playerCamera.Priority = 10;
        }
    }
}
