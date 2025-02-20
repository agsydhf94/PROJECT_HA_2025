using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Cinemachine;

namespace HA
{
    public class ShopUI : UIBase
    {
        public static ShopUI instance;
        public static ShopUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ShopUI>();
                }
                return instance;
            }
        }

        public TMP_Text currentMoneyText;
        public Canvas shopUICanvas;
        public Button[] itemButtons;
        public Button exitButton;
        public string purchasingItemName;
        public GameObject PurchasedMesagePanel;
        public TMP_Text purchasedMessageText;

        public CinemachineVirtualCamera _playerCamera;
        public CinemachineVirtualCamera _npcCamera;


        private void Start()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

            currentMoneyText.text = GameManager.Instance.money.ToString();
            shopUICanvas.planeDistance = -0.5f;
            shopUICanvas.worldCamera = Camera.main;
            

            foreach(var button in itemButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    purchasingItemName = button.transform.GetComponentInChildren<TMP_Text>().text;
                    OnClickPurchase(purchasingItemName);
                });
            }
        }



        public async void OnClickPurchase(string itemName)
        {
            var purChasedSO = Resources.Load<EquipmentSO>($"Scriptable Objects/EquipmentsSOs/{itemName}");
            bool isPurchased = false;

            if(GameManager.Instance.money >= purChasedSO.price)
            {
                InventoryManager.Instance.AddEquipment(purChasedSO, 1);
                GameManager.Instance.money -= purChasedSO.price;
                currentMoneyText.text = GameManager.Instance.money.ToString();
                isPurchased = true;
            }

            await PurchaseMessage(isPurchased, itemName);
        }

        public async UniTask PurchaseMessage(bool isPurchased, string itemName)
        {
            if(isPurchased)
            {
                PurchasedMesagePanel.SetActive(true);
                purchasedMessageText.text = $"{itemName} 장비를 구매하였습니다";

                await UniTask.Delay(1500);

                PurchasedMesagePanel.SetActive(false);
                purchasedMessageText.text = "";
            }
            else
            {
                PurchasedMesagePanel.SetActive(true);
                purchasedMessageText.text = $"잔고가 부족합니다";

                await UniTask.Delay(1500);

                PurchasedMesagePanel.SetActive(false);
                purchasedMessageText.text = "";
            }
        }

        public void OnclickExitShop()
        {
            InteractionUI.Instance.gameObject.SetActive(true);
            shopUICanvas.planeDistance = -0.5f;

            _npcCamera.Priority = 10;
            _playerCamera.Priority = 20;
        }
    }
}
