using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HA
{
    public class InGameScene01 : SceneBase
    {
        public override bool IsAdditiveScene => false;

        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.InGameScene01.ToString(), LoadSceneMode);
            while (!async.isDone)
            {
                yield return null;

                float progress = async.progress / 0.9f;
                LoadingUI.Instance.SetProgress(progress);
            }

            // TODO : Show Ingame UI
            UIManager.Instance.Show<InventoryUI>(UIList.InventoryUI);
            UIManager.Instance.Show<SkillTreeUI>(UIList.SkillTreeUI);
            UIManager.Instance.Show<WeaponQuickMenuUI>(UIList.WeaponQuickMenuUI);
            UIManager.Instance.Show<InteractionUI>(UIList.InteractionUI);
            UIManager.Instance.Show<QuestBookUI>(UIList.QuestBookUI);
            UIManager.Instance.Show<QuestInfoUI>(UIList.QuestInfoUI);
            UIManager.Instance.Show<QuestQuickMenuUI>(UIList.QuestQuickMenuUI);

            // Show Ingame Popup UI
            UIManager.Instance.Show<CheckPopUpUI>(UIList.CheckPopUpUI);

            // User Data Loading
            QuestManager.Instance.QuestBookUI_Initialize();
            //InventoryManager.Instance.InventoryUI_Initialize(InventoryDataModel.Instance.inventoryData);
        }

        public override IEnumerator OnEnd()
        {
            // TODO : Hide Ingame UI
            UIManager.Instance.Hide<InventoryUI>(UIList.InventoryUI);
            UIManager.Instance.Hide<SkillTreeUI>(UIList.SkillTreeUI);
            UIManager.Instance.Hide<WeaponQuickMenuUI>(UIList.WeaponQuickMenuUI);
            UIManager.Instance.Hide<InteractionUI>(UIList.InteractionUI);
            UIManager.Instance.Hide<QuestBookUI>(UIList.QuestBookUI);
            UIManager.Instance.Hide<QuestInfoUI>(UIList.QuestInfoUI);
            UIManager.Instance.Hide<QuestQuickMenuUI>(UIList.QuestQuickMenuUI);

            // Hide Ingame Popup UI
            UIManager.Instance.Hide<CheckPopUpUI>(UIList.CheckPopUpUI);

            yield return null;
        }
    }
}
