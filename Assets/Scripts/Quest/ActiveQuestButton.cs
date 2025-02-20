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

        // 퀘스트 Reward UI 프리팹을 저장하는 장소
        public List<QuestRewardUI_Item> questRewardUI_Of_ThisButton;

        // Caching
        QuestManager questManager;
        QuestBookUI questBookUI;
        QuestQuickMenuUI questQuickMenuUI;

        


        // 퀘스트 버튼이 생성될 때, 퀘스트 진행도 텍스트가 활성되지 않는 문제가 있었음
        // UI 요소가 갱신되기 전에 OnEnable이 호출되어을 가능성을 염두하여 Start로 타이밍 조절
        private IEnumerator Start()
        {
            // 프레임을 기다린 후 진행도 업데이트
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


        // 클릭 시 실행되어, 퀘스트 북 설명창에 정보 표시
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
                Debug.Log($"퀘스트 리워드 프리팹 갯수 : {questRewardUI_Of_ThisButton.Count}개");
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
                Debug.Log($"현재 활성중인 퀘스트 유무 : {questManager.ChildElementBoolChecker()}");
                if(!questManager.ChildElementBoolChecker() && !isQuickMenuSelected)
                {
                    questQuickMenuUI.QuickQuestMenuUI_Reset();
                    questManager.DeselectAllSelectedQuests();
                    isQuickMenuSelected = true;
                    quickMenuSelected.SetActive(true);

                    // 퀘스트 정보 퀵 메뉴로 전송
                    questQuickMenuUI.questID = questData_Of_ThisButton.id;
                    questQuickMenuUI.questName.text = questData_Of_ThisButton.questName;
                    questQuickMenuUI.questDescription.text = questData_Of_ThisButton.questDescription;
                    questQuickMenuUI.questCompletedTextObject.SetActive(questManager.IsQuestCompleted(questData_Of_ThisButton.id));

                    // Progress는 목표 관련 정보가 갱신될 때마다 전송되어야 하기 때문에 여기서 처리하기 곤란
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

        // 퀘스트 진행도 업데이트 시 UI 텍스트 갱신
        public void OnQuestProgressUpdated(int questID, string progressText)
        {
            if (id == questID)
            {
                questBookUI.bookPanel_Activated_QuestProgress.text = progressText;

                // 퀘스트 완료 버튼 상태 갱신
                questBookUI.questCompleteButton.gameObject.SetActive(questManager.IsQuestCompleted(questID));

                
            }
        }

        

    }
}
