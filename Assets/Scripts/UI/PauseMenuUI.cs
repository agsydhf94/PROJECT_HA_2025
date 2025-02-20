using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class PauseMenuUI : UIBase
    {
        public void OnClickResumeGame()
        {
            GameManager.Instance.isPause = false;
            Time.timeScale = 1.0f;
            UIManager.Instance.Hide<PauseMenuUI>(UIList.PauseMenuUI);
        }

        public void OnClickBackToTitle()
        {
            GameManager.Instance.isPause = false;
            Time.timeScale = 1.0f;
            UIManager.Instance.Hide<PauseMenuUI>(UIList.PauseMenuUI);

            // �÷��� ������ ����
            QuestDataModel.Instance.SaveQuestData();
            InventoryDataModel.Instance.SaveInventoryData();

            Main.Instance.ChangeScene(SceneType.TitleScreen);
        }
    }
}
