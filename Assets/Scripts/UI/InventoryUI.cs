using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{

    public class InventoryUI : UIBase
    {
        public Canvas inventoryUI_Canvas;
        public bool isInventoryOn;
        public bool isItemPanelOn;
        public bool isEquipmentPanelOn;
        public CanvasGroup itemMenuCanvasGroup;
        public CanvasGroup equipmentMenuCanvasGroup;

        private void Awake()
        {
            //equipmentMenuCanvasGroup.alpha = 0;
            //itemMenuCanvasGroup.alpha = 0;

            inventoryUI_Canvas.worldCamera = Camera.main;
            inventoryUI_Canvas.planeDistance = -0.5f;

            
        }
        /*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ItemMenuUI();
                inventoryUI_Canvas.planeDistance = 1.3f;
                isInventoryOn = true;

            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                EquipmentUI();
                inventoryUI_Canvas.planeDistance = 1.3f;
                isInventoryOn = true;
            }

            if(isEquipmentPanelOn || isItemPanelOn)
            {
                isInventoryOn = true;
            }
            else
            {
                isInventoryOn= false;
            }

            if(!isInventoryOn)
            {
                inventoryUI_Canvas.planeDistance = -0.5f;
            }
        }

        private void ItemMenuUI()
        {
            isInventoryOn = true;

            if (itemMenuCanvasGroup.alpha == 1)  // 메뉴가 활성화된 상태
            {
                StartCoroutine(UIController.Instance.FadeUI(itemMenuCanvasGroup, 0f, 0.2f));  // 페이드 아웃
                equipmentMenuCanvasGroup.alpha = 0f;  // EquipmentMenu 비활성화
                equipmentMenuCanvasGroup.interactable = false;
                equipmentMenuCanvasGroup.blocksRaycasts = false;
                equipmentMenuCanvasGroup.gameObject.SetActive(false);
                isEquipmentPanelOn = false;


                itemMenuCanvasGroup.interactable = false;
                itemMenuCanvasGroup.blocksRaycasts = false;
                isItemPanelOn = false;
            }
            else
            {
                equipmentMenuCanvasGroup.gameObject.SetActive(true);
                StartCoroutine(UIController.Instance.FadeUI(itemMenuCanvasGroup, 1f, 0.2f));  // 페이드 인
                equipmentMenuCanvasGroup.alpha = 0f;  // EquipmentMenu 비활성화
                equipmentMenuCanvasGroup.interactable = false;
                equipmentMenuCanvasGroup.blocksRaycasts = false;
                isEquipmentPanelOn = false;

                itemMenuCanvasGroup.interactable = true;  // 메뉴 활성화
                itemMenuCanvasGroup.blocksRaycasts = true;
                isItemPanelOn = true;
            }
        }

        private void EquipmentUI()
        {

            if (equipmentMenuCanvasGroup.alpha == 1)  // 메뉴가 활성화된 상태
            {
                StartCoroutine(UIController.Instance.FadeUI(equipmentMenuCanvasGroup, 0f, 0.2f));  // 페이드 아웃
                itemMenuCanvasGroup.alpha = 0f;  // ItemMenu 비활성화
                itemMenuCanvasGroup.interactable = false;
                itemMenuCanvasGroup.blocksRaycasts = false;
                itemMenuCanvasGroup.gameObject.SetActive(false);
                isItemPanelOn = false;


                equipmentMenuCanvasGroup.interactable = false;
                equipmentMenuCanvasGroup.blocksRaycasts = false;
                isEquipmentPanelOn = false;
            }
            else
            {
                itemMenuCanvasGroup.gameObject.SetActive(true);
                StartCoroutine(UIController.Instance.FadeUI(equipmentMenuCanvasGroup, 1f, 0.2f));  // 페이드 인
                itemMenuCanvasGroup.alpha = 0f;  // ItemMenu 비활성화
                itemMenuCanvasGroup.interactable = false;
                itemMenuCanvasGroup.blocksRaycasts = false;
                isItemPanelOn = false;

                equipmentMenuCanvasGroup.interactable = true;  // 메뉴 활성화
                equipmentMenuCanvasGroup.blocksRaycasts = true;
                isEquipmentPanelOn = true;
            }
        }
        */

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventoryMenu(true);  // 아이템 메뉴 열기
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleInventoryMenu(false); // 장비 메뉴 열기
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isInventoryOn)
            {
                CloseInventory(); // ESC 키로 닫기
            }
        }

        private void ToggleInventoryMenu(bool openItemMenu)
        {
            isInventoryOn = !isInventoryOn;

            if (isInventoryOn)
            {
                inventoryUI_Canvas.planeDistance = 1.3f;
                if (openItemMenu)
                {
                    ShowItemMenu();
                }
                else
                {
                    ShowEquipmentMenu();
                }
            }
            else
            {
                CloseInventory();
            }
        }

        private void ShowItemMenu()
        {
            StartCoroutine(ShowMenu(itemMenuCanvasGroup, true));
            StartCoroutine(HideMenu(equipmentMenuCanvasGroup));
            isItemPanelOn = true;
            isEquipmentPanelOn = false;
        }

        private void ShowEquipmentMenu()
        {
            StartCoroutine(ShowMenu(equipmentMenuCanvasGroup, true));
            StartCoroutine(HideMenu(itemMenuCanvasGroup));
            isEquipmentPanelOn = true;
            isItemPanelOn = false;
        }

        private void CloseInventory()
        {
            StartCoroutine(HideMenu(itemMenuCanvasGroup));
            StartCoroutine(HideMenu(equipmentMenuCanvasGroup));
            isInventoryOn = false;
            isItemPanelOn = false;
            isEquipmentPanelOn = false;
            inventoryUI_Canvas.planeDistance = -0.5f;
        }

        private IEnumerator ShowMenu(CanvasGroup menu, bool interactable)
        {
            menu.gameObject.SetActive(true);
            yield return StartCoroutine(UIController.Instance.FadeUI(menu, 1f, 0.2f));
            menu.interactable = interactable;
            menu.blocksRaycasts = interactable;
        }

        private IEnumerator HideMenu(CanvasGroup menu)
        {
            yield return StartCoroutine(UIController.Instance.FadeUI(menu, 0f, 0.2f));
            menu.interactable = false;
            menu.blocksRaycasts = false;
            menu.gameObject.SetActive(false);
        }
    }

    
}
