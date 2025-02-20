using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace HA
{
    public class QuestManager : Singleton<QuestManager>
    {

        // 적을 처치하는 퀘스트에서의 진행도 갱신
        public delegate void OnQuestProgressUpdated(int questID, string progressText);
        public static event OnQuestProgressUpdated QuestProgressUpdated;


        // QuestInfo UI 에서
        // 퀘스트 정보를 띄울 때, 퀘스트 Reward UI 프리팹을 임시로 저장하는 리스트 
        public List<QuestRewardUI_Item> questInfo_questReward_UIs = new List<QuestRewardUI_Item>();

        //public Button questAcceptButton;
        //public Button questCancelButton;        
        //public Transform questBook_ActivatedQuest;
        //public Transform questBook_CompletedQuest;
        // public Button activeQuestButton;

        
        /*
        [Header("QuestBook UI")]
        public GameObject questBook;
        public GameObject selectedActiveQuest_Reward;
        public bool buttonSelected;
        public bool activeQuestQuickMenu;
        */

        // QuestBook 에서
        // 퀘스트 Reward UI 프리팹을 창에 띄우고 지울 때, 임시로 사용하는 리스트
        // 이를 사용하지 않고 GetChild 와 while 문을 썼을 때 발생할 수 있는 무한루프 방지 목적
        public List<QuestRewardUI_Item> questRewardUI_TemporaryStorage = new List<QuestRewardUI_Item>();




        public static Dictionary<int, ActiveQuest> activeQuests = new Dictionary<int, ActiveQuest>();
        public static Dictionary<int, GameObject> activeQuestButtons = new Dictionary<int, GameObject>();
        public static Dictionary<int, EnemyKilledData_byPlayer> enemyKilled_byPlayer = new Dictionary<int, EnemyKilledData_byPlayer>();
        public static Dictionary<int, NPCTalkedData_byPlayer> npctalked_byPlayer = new Dictionary<int, NPCTalkedData_byPlayer>();
        public static List<int> finishedQuests = new List<int>();

        public GameObject popUpUI;
        public Color blinkingColor;


        public Dictionary<int, QuestData> allQuestData = new Dictionary<int, QuestData>();

        private bool isInitialized = false;

        /*
        private void Awake()
        {
            TextAsset jsonText = Resources.Load<TextAsset>("Quest/Quest");
            AllQuestContainer allQuestContainer = JsonUtility.FromJson<AllQuestContainer>(jsonText.text);
            Debug.Log($"모든 퀘스트 로딩 : {allQuestContainer.quests.Count}");
            foreach(QuestData quest in allQuestContainer.quests)
            {
                Debug.Log($"퀘스트 ID : {quest.id}추가됨");
                allQuestData.Add(quest.id ,quest);
            }
        }
        */
        /*
        private void Awake()
        {
            Initialize();

            //CheckSubscribedMethods(QuestProgressUpdated);
        }
        */

        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            // QuestQuickMenuUI.Instance.QuestQuickMenuUI_Initialize();


            TextAsset jsonText = Resources.Load<TextAsset>("Quest/Quest");
            AllQuestContainer allQuestContainer = JsonUtility.FromJson<AllQuestContainer>(jsonText.text);
            Debug.Log($"모든 퀘스트 로딩 : {allQuestContainer.quests.Count}");
            foreach (QuestData quest in allQuestContainer.quests)
            {
                Debug.Log($"퀘스트 ID : {quest.id}추가됨");
                allQuestData.Add(quest.id, quest);
            }
        }



        public string QuestInformation_TaskString(QuestData quest)
        {

            // 본 메서드는 퀘스트 정보를 확인해서 ( 이름, 설명, 미션, 보상 ) 
            // 추후 UI에 넣어줄 수 있도록 스트링을 정리하는 용도로 사용되는 메서드

            string taskString;


            // Task 컨트롤
            taskString = "";

            // 적을 처치하는 퀘스트
            if (quest.questTask.questNeededEnemies != null)
            {
                foreach (QuestKill questNeededEnemy_element in quest.questTask.questNeededEnemies)
                {
                    int currentKills = 0;
                    if (activeQuests.ContainsKey(questNeededEnemy_element.enemyID))
                    {
                        currentKills = enemyKilled_byPlayer[questNeededEnemy_element.enemyID].totalAmount_soFar
                                     - activeQuests[quest.id].activeQuest_Enemies_ToKill[questNeededEnemy_element.enemyID].InitialKillAmount_atStart;
                    }

                        taskString += "Eliminate" + questNeededEnemy_element.enemyAmount_ToKill + " "
                                  + EnemyDatabase.enemyDatabase[questNeededEnemy_element.enemyID] + "\n";
                }
            }

            // 아이템 수집 퀘스트
            if (quest.questTask.questNeededItems != null)
            {
                foreach (QuestItem questNeededItems_element in quest.questTask.questNeededItems)
                {                    
                    /*
                    taskString += "Collect" + questNeededItems_element.itemAmount + " "
                                  + ItemDatabase.itemDatabase[questNeededItems_element.itemID] + "\n";
                    */

                    taskString += "Collect" + questNeededItems_element.itemAmount + " "
                                  + ItemDatabase.itemDatabase[questNeededItems_element.itemID].itemName + "\n";
                }
            }

            // 누군가와 대화하는 퀘스트
            if (quest.questTask.questNeededNPCs != null)
            {
                foreach (NPCToTalk questNeededNPCs_element in quest.questTask.questNeededNPCs)
                {
                    taskString += "Talk to" + NPCDatabase.NPCs[questNeededNPCs_element.idOfNPC_NeededToTalk] + "\n";
                }
            }


            return taskString;

        }

        
        public void QuestInformation_UIDisplay(QuestData quest)
        {
            QuestInfoUI.Instance.questNamePanel.GetComponentInChildren<TMP_Text>().text = quest.questName;
            QuestInfoUI.Instance.questDescriptionPanel.GetComponentInChildren<TMP_Text>().text = quest.questDescription;
            QuestInfoUI.Instance.questTaskPanel.GetComponentInChildren<TMP_Text>().text = QuestInformation_TaskString(quest);
            RewardUIPrefab_Generater(quest);
        }



        // 퀘스트가 있는 NPC와의 대화 말미에 퀘스트 정보 표시
        public void ShowQuestInformation(QuestData quest)
        {
            // 수락 버튼을 누르면 해당 퀘스트를 수락하도록 설정
            QuestInfoUI.Instance.questAcceptButton.onClick.RemoveAllListeners();
            QuestInfoUI.Instance.questAcceptButton.onClick.AddListener(() => {
                Debug.Log("버튼 Listener");
                OnClickQuestAccept(quest);
                });
            Debug.Log("퀘스트 수락 버튼");


            QuestInfoUI.Instance.questCancelButton.onClick.RemoveAllListeners();
            QuestInfoUI.Instance.questCancelButton.onClick.AddListener(() => OnClickQuestDecline());
            Debug.Log("퀘스트 거절 버튼");
       
        }


        public void OnClickQuestAccept(QuestData quest)
        {
            Debug.Log("Quest accepted: " + quest.id); // 퀘스트 ID 확인
            AddQuest(quest.id);
            //QuestInfoUI.Instance.questUIBackground_Panel.SetActive(false);
            QuestInfoUI.Instance.QuestInfoUI_OFF();
            //popUpUI.GetComponent<TextWriting>().StartUIAnimationSequence($"퀘스트 : {quest.questName} 수락되었습니다", blinkingColor);
            CheckPopUpUI.Instance.StartUIAnimationSequence($"퀘스트 : {quest.questName} 수락되었습니다", blinkingColor);
            ActiveQuestTransfer_ToQuestBookUI(quest);


            /*
            var activeQuestButtonPrefab = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Activated_Contents);
            var questInfo = allQuestData[quest.id];
            var questButton_Component = activeQuestButtonPrefab.GetComponent<ActiveQuestButton>();
            questButton_Component.questData_Of_ThisButton = questInfo;

            foreach(var rewardUI in questInfo_questReward_UIs)
            {
                questButton_Component.questRewardUI_Of_ThisButton.Add(rewardUI);
            }

            activeQuestButtons.Add(quest.id, activeQuestButtonPrefab.gameObject);
            questInfo_questReward_UIs.Clear();
            Debug.Log(questInfo.questName);
            */


        }

        public void ActiveQuestTransfer_ToQuestBookUI(QuestData quest)
        {
            //var activeQuestButtonPrefab = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Activated_Contents);
            var questInfo = allQuestData[quest.id];

            var activeQuestButtonPrefab_Resource = Resources.Load("UI/ActiveQuestButtonUI/ActiveQuestButton") as GameObject;
            var activeQuestButtonPrefab = Instantiate(activeQuestButtonPrefab_Resource, QuestBookUI.Instance.bookPanel_Activated_Contents);
            activeQuestButtonPrefab.transform.SetParent(QuestBookUI.Instance.bookPanel_Activated_Contents);

            var questButton_Component = activeQuestButtonPrefab.GetComponent<ActiveQuestButton>();
            questButton_Component.questData_Of_ThisButton = questInfo;

            questButton_Component.ActiveQuestButton_Initialize();
            foreach (var rewardUI in questInfo_questReward_UIs)
            {
                questButton_Component.questRewardUI_Of_ThisButton.Add(rewardUI);
            }

            if (!activeQuestButtons.ContainsKey(quest.id))
            {
                activeQuestButtons.Add(quest.id, activeQuestButtonPrefab);
            }
            questInfo_questReward_UIs.Clear();
            Debug.Log(questInfo.questName);
        }

        public void OnClickQuestDecline()
        {
            // QuestInfoUI.Instance.questUIBackground_Panel.SetActive(false);
            QuestInfoUI.Instance.QuestInfoUI_OFF();
            questInfo_questReward_UIs.Clear();
        }


        public void OnClickCompleteQuest(QuestData quest)
        {
            // 퀘스트 완료 처리 로직 (예: 보상 지급, 퀘스트 목록에서 제거 등)
            QuestReward(quest);
            finishedQuests.Add(quest.id);
            CompletedQuestTransfer_ToQuestBookUI(quest);
            /*
            var completedQuestButton = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Completed_Contents);
            var completedQuestButton_Component = completedQuestButton.GetComponent<ActiveQuestButton>();
            completedQuestButton_Component.questData_Of_ThisButton = quest;
            completedQuestButton_Component.id = quest.id;

            Debug.Log($"액티브 퀘스트 안에 있는 UI 프리팹 개수 : {activeQuests[quest.id].activeQuest_RewardUIPrefab[quest.id].Count}");
            foreach(var rewardUI in activeQuests[quest.id].activeQuest_RewardUIPrefab[quest.id])
            {
                completedQuestButton_Component.questRewardUI_Of_ThisButton.Add(rewardUI);
            }

            var activeQuestButton_AboutToRemove = activeQuestButtons[quest.id].GetComponent<ActiveQuestButton>();
            RewardUIPrefab_Reset(activeQuestButton_AboutToRemove.questRewardUI_Of_ThisButton, QuestBookUI.Instance.bookPanel_Activated_QuestReward.transform);
            
            Destroy(activeQuestButtons[quest.id]);
            activeQuestButtons.Remove(quest.id);

            RemoveQuest(quest.id);
            */


            // 완료 후 UI 갱신
            // QuestInfoUI.Instance.questUIBackground_Panel.SetActive(false);
            QuestInfoUI.Instance.QuestInfoUI_OFF();
            QuestQuickMenuUI.Instance.QuickQuestMenuUI_Reset();


            // ActiveQuestButton_DataTrasnsfer(); // 갱신된 퀘스트 목록 표시
            // 이 부분은 차후 완료된 퀘스트 북에 넘어가는 부분으로 만듬
        }

        public void CompletedQuestTransfer_ToQuestBookUI(QuestData quest)
        {
            // var completedQuestButton = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Completed_Contents);
            var completedQuestButton_Resource = Resources.Load("UI/ActiveQuestButtonUI/ActiveQuestButton") as GameObject;
            var completedQuestButton = Instantiate(completedQuestButton_Resource, QuestBookUI.Instance.bookPanel_Activated_Contents);
            completedQuestButton.transform.SetParent(QuestBookUI.Instance.bookPanel_Completed_Contents);

            var completedQuestButton_Component = completedQuestButton.GetComponent<ActiveQuestButton>();
            completedQuestButton_Component.questData_Of_ThisButton = quest;
            completedQuestButton_Component.id = quest.id;

            Debug.Log($"액티브 퀘스트 안에 있는 UI 프리팹 개수 : {activeQuests[quest.id].activeQuest_RewardUIPrefab[quest.id].Count}");
            foreach (var rewardUI in activeQuests[quest.id].activeQuest_RewardUIPrefab[quest.id])
            {
                completedQuestButton_Component.questRewardUI_Of_ThisButton.Add(rewardUI);
            }

            var activeQuestButton_AboutToRemove = activeQuestButtons[quest.id].GetComponent<ActiveQuestButton>();
            RewardUIPrefab_Reset(activeQuestButton_AboutToRemove.questRewardUI_Of_ThisButton, QuestBookUI.Instance.bookPanel_Activated_QuestReward.transform);

            Destroy(activeQuestButtons[quest.id]);
            activeQuestButtons.Remove(quest.id);

            RemoveQuest(quest.id);
        }

        public void QuestReward(QuestData quest)
        {
            if(quest.questReward.rewardExp > 0)
            {
                GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().exp += quest.questReward.rewardExp;
            }

            if(quest.questReward.rewardMoney > 0)
            {
                GameManager.Instance.money += quest.questReward.rewardMoney;
            }

            if(quest.questReward.questItems.Length > 0)
            {
                foreach(var rewardItem in  quest.questReward.questItems)
                {
                    if(ItemDatabase.itemDatabase.TryGetValue(rewardItem.itemID, out ItemSO itemSO))
                    {
                        InventoryManager.Instance.AddItem(itemSO, rewardItem.itemAmount);
                    }
                }
            }
        }





        public bool IsQuestCompleted(int questID)
        {
            Debug.Log($"퀘스트 아이디 : {questID}");
            QuestData quest = allQuestData[questID];

            Debug.Log(quest.questTask.questNeededEnemies.Length);

            // 처치 퀘스트일 경우 퀘스트 완료 판단
            if(quest.questTask.questNeededEnemies.Length > 0)
            {
                foreach(var questKill in quest.questTask.questNeededEnemies)
                {
                    if (!enemyKilled_byPlayer.ContainsKey(questKill.enemyID))
                    {
                        return false;
                    }

                    int currentKills = enemyKilled_byPlayer[questKill.enemyID].totalAmount_soFar
                                     - activeQuests[quest.id].activeQuest_Enemies_ToKill[questKill.enemyID].InitialKillAmount_atStart;
                    
                    if(currentKills < questKill.enemyAmount_ToKill)
                    {
                        return false;
                    }
                }
            }

            // 누군가와 대화해야하는 퀘스트일 경우의 판단
            if(quest.questTask.questNeededNPCs.Length > 0)
            {
                foreach(var NPC in quest.questTask.questNeededNPCs)
                {
                    if (!activeQuests[quest.id].nPCToTalk[NPC.idOfNPC_NeededToTalk].readyToComplete)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }






        public void UpdateQuestProgress_Main(int questID)
        {
            QuestData quest = allQuestData[questID];

            // 적 처치 퀘스트가 맞는지 확인
            if (quest.questTask.questNeededEnemies != null)
            {
                foreach (QuestKill questKill in quest.questTask.questNeededEnemies)
                {
                    // 진행도 업데이트
                    string progressString = UpdateQuestProGress(questID);
                    Debug.Log(progressString);

                    Debug.Log(QuestProgressUpdated);
                    // 이벤트로 진행도 업데이트를 알림
                    if (QuestProgressUpdated != null)
                    {
                        QuestProgressUpdated(questID, progressString);
                    }
                }
            }

            if(quest.questTask.questNeededNPCs != null)
            {
                foreach(var element in quest.questTask.questNeededNPCs)
                {
                    string progressString = UpdateQuestProGress(questID);
                    if(QuestProgressUpdated != null)
                    {
                        QuestProgressUpdated(questID, progressString);
                    }
                }
            }
        }

        public string UpdateQuestProGress(int questID)
        {
            QuestData quest = allQuestData[questID];
            string progressString = "";
            Debug.Log("progressString 진입");

            if (quest.questTask.questNeededEnemies != null)
            {
                Debug.Log(quest.questTask.questNeededEnemies);
                foreach (QuestKill questKill in quest.questTask.questNeededEnemies)
                {
                    // 아카이빙 - 초기 마크 를 계산해서 진행중인 퀘스트의 순 진행도를 계산
                    int currentKills = enemyKilled_byPlayer.ContainsKey(questKill.enemyID)
                        ? enemyKilled_byPlayer[questKill.enemyID].totalAmount_soFar
                          - activeQuests[quest.id].activeQuest_Enemies_ToKill[questKill.enemyID].InitialKillAmount_atStart
                        : 0;
                    
                    progressString += $"{currentKills}/{questKill.enemyAmount_ToKill} "
                        + $"{EnemyDatabase.enemyDatabase[questKill.enemyID]}\n";
                    
                }
            }

            if(quest.questTask.questNeededNPCs != null)
            {
                foreach(var element in quest.questTask.questNeededNPCs)
                {
                    string readyToCompleteStr = "";
                    if (activeQuests[quest.id].nPCToTalk[element.idOfNPC_NeededToTalk].readyToComplete)
                    {
                        readyToCompleteStr = "완료";
                    }
                    else
                    {
                        readyToCompleteStr = "미완료";
                    }

                    progressString += $"{NPCDatabase.NPCs[element.idOfNPC_NeededToTalk]} 과 대화한다 : {readyToCompleteStr}";
                }
                
            }
            return progressString;
        }



        public void AddQuest(int questID)
        {
            if (activeQuests.ContainsKey(questID))
            {
                return;
            }
            Debug.Log(questID);
            // 퀘스트 객체 추가
            QuestData questData = QuestManager.Instance.allQuestData[questID];
            ActiveQuest newActiveQuest = new ActiveQuest();
            newActiveQuest.activeQuest_ID = questID;
            newActiveQuest.activeQuest_Date = DateTime.Now.ToLongDateString();


            // 처치관련 요소가 있는 퀘스트라면
            if (questData.questTask.questNeededEnemies.Length > 0)
            {
                // 퀘스트에서 처치해야 할 적 정보(QuestKill[])를 저장할 배열 초기화
                // questNeededKills.Length 는 처치해야 하는 적의 종류의 수
                // newActiveQuest.activeQuest_Enemies_ToKill = new QuestKill[questData.questTask.questNeededEnemies.Length];

                // 퀘스트 목표에 해당하는 각각의 적 정보를 처리
                foreach (QuestKill questNeededEnemy_element in questData.questTask.questNeededEnemies)
                {

                    // 해당 적의 ID를 사용해 새로운 QuestKill 객체를 생성하여 저장
                    newActiveQuest.activeQuest_Enemies_ToKill[questNeededEnemy_element.enemyID] = new QuestKill();

                    // 적 ID가 enemyKilled 딕셔너리에 없는 경우 새로 추가 (처치 수를 추적하기 위함)
                    if (!enemyKilled_byPlayer.ContainsKey(questNeededEnemy_element.enemyID))
                    {
                        enemyKilled_byPlayer.Add(questNeededEnemy_element.enemyID, new EnemyKilledData_byPlayer());
                    }

                    // 퀘스트가 시작하면, InitialKillAmount_atStart에
                    // 지금까지 잡은 해당 적(아카이빙 용)의 수를 마크한다
                    newActiveQuest.activeQuest_Enemies_ToKill[questNeededEnemy_element.enemyID].enemyID = questNeededEnemy_element.enemyID;
                    newActiveQuest.activeQuest_Enemies_ToKill[questNeededEnemy_element.enemyID].enemyAmount_ToKill = questNeededEnemy_element.enemyAmount_ToKill;
                    newActiveQuest.activeQuest_Enemies_ToKill[questNeededEnemy_element.enemyID].InitialKillAmount_atStart = enemyKilled_byPlayer[questNeededEnemy_element.enemyID].totalAmount_soFar;
                }
            }


            // NPC와 대화하는 퀘스트라면
            if (questData.questTask.questNeededNPCs.Length > 0)
            {
                // 퀘스트에서 대화해야 할 NPC 정보를 저장할 배열 초기화
                // newActiveQuest.nPCToTalk = new NPCToTalk[questData.questTask.questNeededNPCs.Length];
                newActiveQuest.nPCToTalk = new Dictionary<int, NPCToTalk>();

                // 각 NPC 정보를 처리
                foreach (NPCToTalk npcToTalk in questData.questTask.questNeededNPCs)
                {

                    // NPC ID를 기준으로 새로운 NPCToTalk 객체를 생성하여 저장
                    // newActiveQuest.nPCToTalk[npcToTalk.idOfNPC_NeededToTalk] = new NPCToTalk();
                    newActiveQuest.nPCToTalk.Add(npcToTalk.idOfNPC_NeededToTalk, new NPCToTalk());

                    // 대화 퀘스트가 완료가 준비된 상태로 설정
                    // 처음 퀘스트 받을 때, 기록을 받아두고 나중에 목표 NPC와 대화할 때 확인 받는 방식
                    newActiveQuest.nPCToTalk[npcToTalk.idOfNPC_NeededToTalk].readyToComplete = false;
                }
            }
            if(!activeQuests.ContainsKey(questID))
            {
                // newActiveQuest.activeQuest_RewardUIPrefab.Add(questData.id, questInfo_questReward_UIs);

                // ActiveQuest의 UI프리팹 저장소 리스트에 Key가 없으면
                // 해당 Key에 대응하는 원소인 리스트를 먼저 초기화
                if (!newActiveQuest.activeQuest_RewardUIPrefab.ContainsKey(questData.id))
                {
                    newActiveQuest.activeQuest_RewardUIPrefab[questData.id] = new List<QuestRewardUI_Item>();
                }

                foreach (var rewardUI in questInfo_questReward_UIs)
                {
                    newActiveQuest.activeQuest_RewardUIPrefab[questData.id].Add(rewardUI);
                }

                activeQuests.Add(questID, newActiveQuest);
            }
            
            
        }

        public void RewardUIPrefab_Generater(QuestData questData)
        {
            
            if(questData.questReward.rewardExp > 0)
            {                
                var rewardUI_Resource = Resources.Load("UI/UIPrefabs/Reward UI_Item_Exp") as GameObject;
                var rewardUIprefab = Instantiate(rewardUI_Resource, QuestInfoUI.Instance.questRewardPrefabPanel.transform);
                QuestRewardUI_Item rewardUI = rewardUIprefab.GetComponent<QuestRewardUI_Item>();

                rewardUI.itemName.text = questData.questReward.rewardExp.ToString() + "Exp";
                questInfo_questReward_UIs.Add(rewardUI);
            }

            if(questData.questReward.rewardMoney > 0)
            {
                var rewardUI_Resource = Resources.Load("UI/UIPrefabs/Reward UI_Item_Money") as GameObject;
                var rewardUIprefab = Instantiate(rewardUI_Resource, QuestInfoUI.Instance.questRewardPrefabPanel.transform);
                QuestRewardUI_Item rewardUI = rewardUIprefab.GetComponent<QuestRewardUI_Item>();

                rewardUI.itemName.text = questData.questReward.rewardMoney.ToString() + "CP";
                questInfo_questReward_UIs.Add(rewardUI);
            }
            
            if(questData.questReward.questItems.Length > 0)
            {
                foreach(var rewardItem in  questData.questReward.questItems)
                {
                    var rewardUI_Resource = Resources.Load("UI/UIPrefabs/Reward UI_Item") as GameObject;
                    var rewardUIprefab = Instantiate(rewardUI_Resource, QuestInfoUI.Instance.questRewardPrefabPanel.transform);
                    QuestRewardUI_Item rewardUI = rewardUIprefab.GetComponent<QuestRewardUI_Item>();

                    rewardUI.itemImage.sprite = ItemDatabase.itemDatabase[rewardItem.itemID].itemSprite;
                    rewardUI.itemName.text = ItemDatabase.itemDatabase[rewardItem.itemID].itemName;
                    rewardUI.quantityText.text = rewardItem.itemAmount.ToString();
                    questInfo_questReward_UIs.Add(rewardUI);
                }
            }


        }


        public void RewardUIPrefab_Reset(List<QuestRewardUI_Item> target_QuestRewardUI, Transform targetParent)
        {
            if(questRewardUI_TemporaryStorage.Count > 0)
            {
                foreach (var rewardUI in questRewardUI_TemporaryStorage)
                {
                    Destroy(rewardUI.gameObject);
                }

                questRewardUI_TemporaryStorage.Clear();

                foreach (var rewardUI in target_QuestRewardUI)
                {
                    var instanceOf_rewardUI = Instantiate(rewardUI, targetParent);
                    questRewardUI_TemporaryStorage.Add(instanceOf_rewardUI);
                }
            }            
        }

        

        public static void RemoveQuest(int questId)
        {
            // 만약 activeQuests에 해당 퀘스트가 있다면
            if (activeQuests.ContainsKey(questId))
            {
                // 해당 퀘스트를 activeQuests에서 제거
                activeQuests.Remove(questId);
            }
        }

        
        public void DeselectAllButtons()
        {
            foreach(var key in activeQuestButtons.Keys)
            {
                activeQuestButtons[key].GetComponent<ActiveQuestButton>().buttonSelected.SetActive(false);
                activeQuestButtons[key].GetComponent<ActiveQuestButton>().isButtonSelected = false;
            }
        }

        public void DeselectAllSelectedQuests()
        {
            foreach (var key in activeQuestButtons.Keys)
            {
                activeQuestButtons[key].GetComponent<ActiveQuestButton>().quickMenuSelected.SetActive(false);
                activeQuestButtons[key].GetComponent<ActiveQuestButton>().isQuickMenuSelected = false;
            }
        }

        public bool ChildElementBoolChecker()
        {
            bool result = false;
            int count = 0;

            foreach (var key in activeQuestButtons.Keys)
            {
                if(activeQuestButtons[key].GetComponent<ActiveQuestButton>().isQuickMenuSelected)
                {
                    count++;
                }
            }

            if(count > 1)
            {
                result = true;
            }

            return result;
        }


        public void QuestBookUI_Initialize()
        {
            Debug.Log("퀘스트 북 UI 갱신 시작");

            // 기존 UI 요소 초기화
            if(QuestBookUI.Instance.bookPanel_Activated_Contents.transform.childCount > 0)
            {
                foreach (Transform child in QuestBookUI.Instance.bookPanel_Activated_Contents)
                {
                    Destroy(child.gameObject);
                }
            }
            
            if(QuestBookUI.Instance.bookPanel_Completed_Contents.childCount > 0)
            {
                foreach (Transform child in QuestBookUI.Instance.bookPanel_Completed_Contents)
                {
                    Destroy(child.gameObject);
                }
            }
            

            // 진행 중인 퀘스트 버튼 추가
            if(activeQuests.Count > 0)
            {
                foreach (var quest in activeQuests.Values)
                {
                    ActiveQuestTransfer_ToQuestBookUI(allQuestData[quest.activeQuest_ID]);

                    activeQuestButtons[quest.activeQuest_ID].GetComponent<ActiveQuestButton>().OnClickShowActiveQuestInfo();
                    /*
                    var questButton = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Activated_Contents);
                    var questButtonComponent = questButton.GetComponent<ActiveQuestButton>();

                    questButtonComponent.questData_Of_ThisButton = allQuestData[quest.activeQuest_ID];
                    questButtonComponent.id = quest.activeQuest_ID;

                    // 퀘스트 진행 정보 표시
                    questButtonComponent.OnClickShowActiveQuestInfo();

                    activeQuestButtons.Add(quest.activeQuest_ID, questButton.gameObject);
                    */
                }
            }
            
            if(finishedQuests.Count > 0)
            {
                // 완료된 퀘스트 버튼 추가
                foreach (var questID in finishedQuests)
                {
                    CompletedQuestTransfer_ToQuestBookUI(allQuestData[questID]);

                    /*
                    var completedQuestButton = Instantiate(activeQuestButton, QuestBookUI.Instance.bookPanel_Completed_Contents);
                    var completedQuestButtonComponent = completedQuestButton.GetComponent<ActiveQuestButton>();

                    completedQuestButtonComponent.questData_Of_ThisButton = allQuestData[questID];

                    activeQuestButtons.Add(questID, completedQuestButton.gameObject);
                    */
                }
            }
            

            Debug.Log("퀘스트 북 UI 갱신 완료");
        }



        // 겹치는 UI 확인용
        public void IsPointerOverUIObject()
        {
            PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

            a_EDCurPos.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(a_EDCurPos, results);
            // return (0 < result.Count);

            if(results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    Debug.Log(i + ":" + results[i].gameObject.name);
                }
            }
        }

        public void CheckSubscribedMethods()
        {
            if (QuestProgressUpdated == null)
            {
                Debug.Log("델리게이트에 등록된 함수가 없습니다.");
                return;
            }

            foreach (Delegate d in QuestProgressUpdated.GetInvocationList())
            {
                Debug.Log($"등록된 함수: {d.Method.Name} (클래스: {d.Target})");
            }
        }



    }

    public class EnemyKilledData_byPlayer
    {
        public int id;
        public int totalAmount_soFar;
    }

    public class NPCTalkedData_byPlayer
    {
        //public int NPCid;
        //public bool flagFromFirstPerson;

        public int talkedSessionCount;
    }


    public class AllQuestContainer
    {
        public List<QuestData> quests = new List<QuestData>();
    }



    public class ActiveQuest
    {
        public int activeQuest_ID;
        public string activeQuest_Date;
        public Dictionary<int, QuestKill> activeQuest_Enemies_ToKill = new Dictionary<int, QuestKill>();
        public Dictionary<int, NPCToTalk> nPCToTalk = new Dictionary<int, NPCToTalk>();
        public Dictionary<int, List<QuestRewardUI_Item>> activeQuest_RewardUIPrefab = new Dictionary<int, List<QuestRewardUI_Item>>();

        // 딕셔너리에 UI프리팹이 들어가는 순간은 Accept 버튼을 누를 때.
        // 그러나, 대화가 끝날 때, 정보창에 띄워주는 역할
    }





}
