using UnityEngine;
using System.IO;
using System.Linq;

namespace HA
{
    public class QuestDataModel : Singleton<QuestDataModel>
    {
        private string savePath => Application.persistentDataPath + "/quest_save.json";
        private bool isInitialized = false;

        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            LoadQuestData();
        }

        public void SaveQuestData()
        {
            QuestDataDTO saveDTO = new QuestDataDTO();

            // 완료된 퀘스트 저장
            saveDTO.finishedQuests = QuestManager.finishedQuests;

            // 현재까지 대화한 NPC 정보 저장
            foreach(var element in QuestManager.npctalked_byPlayer)
            {
                NPCTalkedDTO nPCTalkedDTO = new NPCTalkedDTO();
                nPCTalkedDTO.npcID = element.Key;
                nPCTalkedDTO.talkedSessionCount = element.Value.talkedSessionCount;

                saveDTO.npctalked_byPlayer.Add(nPCTalkedDTO);
            }

            // 진행 중인 퀘스트 저장
            foreach (var questPair in QuestManager.activeQuests)
            {
                ActiveQuest activeQuest = questPair.Value;
                ActiveQuestDTO questDTO = new ActiveQuestDTO
                {
                    questID = activeQuest.activeQuest_ID,
                    startDate = activeQuest.activeQuest_Date
                };

                // 적 처치 진행 상황 저장
                foreach (var kill in activeQuest.activeQuest_Enemies_ToKill)
                {
                    questDTO.killProgress.Add(new KillProgressDTO
                    {
                        enemyID = kill.Value.enemyID,
                        totalAmount_SoFar = QuestManager.enemyKilled_byPlayer[kill.Key].totalAmount_soFar
                    });
                }

                // NPC 대화 진행 상황 저장
                foreach (var npc in activeQuest.nPCToTalk)
                {
                    questDTO.npcProgress.Add(new NPCProgressDTO
                    {
                        npcID = npc.Key,
                        talked = npc.Value.readyToComplete
                    });
                }

                saveDTO.activeQuests.Add(questDTO);
            }

            // JSON 직렬화 및 저장
            string json = JsonUtility.ToJson(saveDTO, true);
            File.WriteAllText(savePath, json);
            Debug.Log("퀘스트 데이터 저장 완료: " + savePath);
        }



        public void LoadQuestData()
        {
            string savePath = Application.persistentDataPath + "/quest_save.json";

            if (!File.Exists(savePath))
            {
                Debug.LogWarning("저장된 퀘스트 데이터 없음");
                return;
            }

            string json = File.ReadAllText(savePath);
            QuestDataDTO loadedDTO = JsonUtility.FromJson<QuestDataDTO>(json);

            // 완료된 퀘스트 로드
            QuestManager.finishedQuests = loadedDTO.finishedQuests;

            // 현재까지 대화한 NPC 정보 로드
            foreach(var element in loadedDTO.npctalked_byPlayer)
            {
                NPCTalkedData_byPlayer nPCTalkedData_ByPlayer = new NPCTalkedData_byPlayer();
                nPCTalkedData_ByPlayer.talkedSessionCount = element.talkedSessionCount;

                QuestManager.npctalked_byPlayer[element.npcID] = nPCTalkedData_ByPlayer;
            }

            // 진행 중인 퀘스트 로드
            foreach (var savedQuest in loadedDTO.activeQuests)
            {
                QuestData questData = QuestManager.Instance.allQuestData[savedQuest.questID];
                ActiveQuest activeQuest = new ActiveQuest
                {
                    activeQuest_ID = savedQuest.questID,
                    activeQuest_Date = savedQuest.startDate
                };

                // 적 처치 진행도 로드
                foreach (var kill in savedQuest.killProgress)
                {
                    activeQuest.activeQuest_Enemies_ToKill[kill.enemyID] = new QuestKill
                    {
                        enemyID = kill.enemyID,
                        enemyAmount_ToKill = questData.questTask.questNeededEnemies
                            .FirstOrDefault(q => q.enemyID == kill.enemyID).enemyAmount_ToKill
                    };

                    QuestManager.enemyKilled_byPlayer[kill.enemyID] = new EnemyKilledData_byPlayer
                    {
                        id = kill.enemyID,
                        totalAmount_soFar = kill.totalAmount_SoFar
                    };
                }

                // NPC 대화 진행도 로드
                foreach (var npc in savedQuest.npcProgress)
                {
                    activeQuest.nPCToTalk[npc.npcID] = new NPCToTalk
                    {
                        idOfNPC_NeededToTalk = npc.npcID,
                        readyToComplete = npc.talked
                    };
                }

                QuestManager.activeQuests.Add(savedQuest.questID, activeQuest);
            }

            Debug.Log("퀘스트 데이터 로드 완료");
        }
    }
}
