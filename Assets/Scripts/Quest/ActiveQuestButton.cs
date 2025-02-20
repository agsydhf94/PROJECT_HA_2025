using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HA
{
    public class ActiveQuestButton: MonoBehaviour, IPointerClickHandler
    {
        
        public int id;

        [Header("Book Panel - Activated Quest")]
        [SerializeField]
        public TMP_Text bookPanel_Activated_PanelMenu_QuestName;


        [Header("Book Panel - Completed Quest")]
        [SerializeField]
        public TMP_Text bookPanel_Completed_PanelMenu_QuestName;

        public GameObject buttonSelected;
        public bool isButtonSelected;
        public GameObject quickMenuSelected;
        public bool isQuickMenuSelected;
        public QuestData questData_Of_ThisButton;

        // ����Ʈ Reward UI �������� �����ϴ� ���
        public List<QuestRewardUI_Item> questRewardUI_Of_ThisButton;

        // Caching
        QuestManager questManager;
        QuestBookUI questBookUI;
        QuestQuickMenuUI questQuickMenuUI;

        


        // ����Ʈ ��ư�� ������ ��, ����Ʈ ���൵ �ؽ�Ʈ�� Ȱ������ �ʴ� ������ �־���
        // UI ��Ұ� ���ŵǱ� ���� OnEnable�� ȣ��Ǿ��� ���ɼ��� �����Ͽ� Start�� Ÿ�̹� ����
        private IEnumerator Start()
        {
            // �������� ��ٸ� �� ���൵ ������Ʈ
            yield return new WaitForEndOfFrame();

            QuestManager.QuestProgressUpdated += OnQuestProgressUpdated;

            
        }

        private void Awake()
        {
            questManager = QuestManager.Instance;
            questBookUI = QuestBookUI.Instance;
            questQuickMenuUI = QuestQuickMenuUI.Instance;            
        }

        public void ActiveQuestButton_Initialize()
        {
            id = questData_Of_ThisButton.id;
            bookPanel_Activated_PanelMenu_QuestName.text = questData_Of_ThisButton.questName;
            Debug.Log(bookPanel_Activated_PanelMenu_QuestName.text);

            if (QuestManager.finishedQuests.Contains(id))
            {
                id = questData_Of_ThisButton.id;
                bookPanel_Completed_PanelMenu_QuestName.text = questData_Of_ThisButton.questName;
            }
        }


        private void OnDisable()
        {
            QuestManager.QuestProgressUpdated -= OnQuestProgressUpdated;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClickLeft();
            }
        }


        // Ŭ�� �� ����Ǿ�, ����Ʈ �� ����â�� ���� ǥ��
        public void OnClickShowActiveQuestInfo()
        {

            if(!QuestManager.finishedQuests.Contains(questData_Of_ThisButton.id))
            {
                questManager.RewardUIPrefab_Reset(questRewardUI_Of_ThisButton, questBookUI.bookPanel_Activated_QuestReward.transform);

                questBookUI.bookPanel_Activated_QuestName.text = questData_Of_ThisButton.questName;
                questBookUI.bookPanel_Activated_QuestDescription.text = questData_Of_ThisButton.questDescription;
                questBookUI.bookPanel_Activated_QuestTask.text = questManager.QuestInformation_TaskString(questData_Of_ThisButton);
                questBookUI.bookPanel_Activated_QuestProgress.text = questManager.UpdateQuestProGress(id);

                questBookUI.questCompleteButton.gameObject.SetActive(questManager.IsQuestCompleted(id));
                questBookUI.questCompleteButton.onClick.RemoveAllListeners();
                questBookUI.questCompleteButton.onClick.AddListener(() => questManager.OnClickCompleteQuest(questData_Of_ThisButton));
            }
            else
            {
                Debug.Log($"����Ʈ ������ ������ ���� : {questRewardUI_Of_ThisButton.Count}��");
                questManager.RewardUIPrefab_Reset(questRewardUI_Of_ThisButton, questBookUI.bookPanel_Completed_QuestReward.transform);

                questBookUI.bookPanel_Completed_QuestName.text = questData_Of_ThisButton.questName;
                questBookUI.bookPanel_Completed_QuestDescription.text = questData_Of_ThisButton.questDescription;
                questBookUI.bookPanel_Compledted_QuestTask.text = questManager.QuestInformation_TaskString(questData_Of_ThisButton);
            }
            
            
            

        }

        public void OnClickLeft()
        {
            if (!isButtonSelected)
            {
                questManager.DeselectAllButtons();
                isButtonSelected = true;
                buttonSelected.SetActive(true);
            }
            else if(isButtonSelected)
            {
                Debug.Log($"���� Ȱ������ ����Ʈ ���� : {questManager.ChildElementBoolChecker()}");
                if(!questManager.ChildElementBoolChecker() && !isQuickMenuSelected)
                {
                    questQuickMenuUI.QuickQuestMenuUI_Reset();
                    questManager.DeselectAllSelectedQuests();
                    isQuickMenuSelected = true;
                    quickMenuSelected.SetActive(true);

                    // ����Ʈ ���� �� �޴��� ����
                    questQuickMenuUI.questID = questData_Of_ThisButton.id;
                    questQuickMenuUI.questName.text = questData_Of_ThisButton.questName;
                    questQuickMenuUI.questDescription.text = questData_Of_ThisButton.questDescription;
                    questQuickMenuUI.questCompletedTextObject.SetActive(questManager.IsQuestCompleted(questData_Of_ThisButton.id));

                    // Progress�� ��ǥ ���� ������ ���ŵ� ������ ���۵Ǿ�� �ϱ� ������ ���⼭ ó���ϱ� ���
                    questQuickMenuUI.questProgress.text = questManager.UpdateQuestProGress(id);

                }
                else if(isQuickMenuSelected)
                {
                    isQuickMenuSelected = false;
                    quickMenuSelected.SetActive(false);

                    questQuickMenuUI.QuickQuestMenuUI_Reset();

                }

            }
        }

        // ����Ʈ ���൵ ������Ʈ �� UI �ؽ�Ʈ ����
        public void OnQuestProgressUpdated(int questID, string progressText)
        {
            if (id == questID)
            {
                questBookUI.bookPanel_Activated_QuestProgress.text = progressText;

                // ����Ʈ �Ϸ� ��ư ���� ����
                questBookUI.questCompleteButton.gameObject.SetActive(questManager.IsQuestCompleted(questID));

                
            }
        }

        

    }
}
