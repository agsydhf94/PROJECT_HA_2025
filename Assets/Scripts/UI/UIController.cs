using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{

    public class UIController : Singleton<UIController>
    {        
        public Canvas settingsCanvas;
        public Slider controlMainVolume;

        public CanvasGroup itemMenuCanvasGroup;  // ItemMenu의 CanvasGroup 참조
        public CanvasGroup equipmentMenuCanvasGroup;  // EquipmentMenu의 CanvasGroup 참조

        private bool isCenterAimOn = false;

        private void Update()
        {
            if (Input.GetMouseButton(1) &&
                GameManager.Instance.currentActiveCharacter.GetComponent<PlayerController>().isArmed &&
                GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentWeapon.weaponSO.weaponCategory == WeaponCategory.MissileLauncher)
            {
                if(!isCenterAimOn)
                {
                    TweenUIController.Instance.ShowAimCircle();
                    Debug.Log("ShowAimCircle 작동");
                    //isCenterAimOn = true;
                }

                Debug.Log("정상 작동!");
                var missileLauncher = WeaponManager.Instance.GetCurrentWeapon<Weapon_MissileLauncher>();
                missileLauncher.UpdateTargetList();
            }
            else
            {
                TweenUIController.Instance.HideAimCircle();
                isCenterAimOn = false;
            }
        }


        // 페이드 인/아웃 애니메이션 코루틴
        public IEnumerator FadeUI(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }


        // 설정 패널 표시
        public void DisplaySettings()
        {
            GameMaster.instance.Display_Settings =
                !GameMaster.instance.Display_Settings;

            settingsCanvas.gameObject.SetActive(GameMaster.instance.Display_Settings);
        }

        public void MainVolume()
        {
            GameMaster.instance.MasterVolume(controlMainVolume.value);
        }
    }
}