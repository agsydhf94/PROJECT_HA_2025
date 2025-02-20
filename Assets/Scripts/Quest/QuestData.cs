using ExcelDataReader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class QuestData
    {
        public int id;
        public string questName;
        public string questDescription;
        public string questRecipent;
        public string questRequiredLevel;
        public QuestReward questReward;
        public Task questTask;
        public Prerequisites prerequisites;
    }

    [System.Serializable]
    public class QuestReward
    {
        public float rewardExp;
        public float rewardMoney;
        public QuestItem[] questItems;
    }

    [System.Serializable]
    public class QuestItem
    {
        public int itemID;
        public int itemAmount;
    }

    [System.Serializable]
    public class Task
    {
        public NPCToTalk[] questNeededNPCs;
        public QuestItem[] questNeededItems;
        public QuestKill[] questNeededEnemies;
    }

    [System.Serializable]
    public class NPCToTalk
    {
        public int idOfNPC_NeededToTalk;
        public bool readyToComplete;
    }


    [System.Serializable]
    public class QuestKill
    {
        public int enemyID;
        public int enemyAmount_ToKill;
        public int InitialKillAmount_atStart;
    }

    [System.Serializable]
    public class Prerequisites
    {
        public int talkedNPC_id;
        public int completedQuestID;
    }


    [System.Serializable]
    public class QuestDataContainer
    {
        public List<QuestData> quests;
    }


    [System.Serializable]
    public class QuestLoader
    {
        public static QuestData LoadQuest(int questID)
        {
            //Json 로딩
            TextAsset jsonText = Resources.Load<TextAsset>("Quest/Quest");
            Debug.Log(jsonText);

            // 로딩된 Json을 QuestDataContainer 타입으로 변환
            QuestDataContainer questData = JsonUtility.FromJson<QuestDataContainer>(jsonText.text);
            Debug.Log(questData.quests);

            //questData를 돌며 입력된 questID에 해당하는 퀘스트 검색
            foreach(var quest in questData.quests)
            {
                if(quest.id == questID)
                {
                    return quest;
                }
            }

            return null;
        }


        public bool QuestData_ConvertExcelToJSON(string excelPath, string jsonPath)
        {
            if (!File.Exists(excelPath))
            {
                Debug.LogError("엑셀 파일이 없습니다.");
                return false;
            }

            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet();
                var questsTable = dataSet.Tables["Quests"];
                var rewardsTable = dataSet.Tables["QuestReward"];
                var tasksTable = dataSet.Tables["QuestTask"];
                var prerequisitesTable = dataSet.Tables["Prerequisites"];

                List<QuestData> quests = new List<QuestData>();

                foreach (DataRow questRow in questsTable.Rows)
                {
                    int questID = int.Parse(questRow["ID"].ToString());
                    QuestData questData = new QuestData();
                    {
                        questData.id = questID;
                        questData.questName = questRow["QuestName"].ToString();
                        questData.questDescription = questRow["QuestDescription"].ToString();
                        questData.questRecipent = questRow["QuestRecipent"].ToString();
                        questData.questRequiredLevel = questRow["QuestRequiredLevel"].ToString();
                        questData.questReward = ParseReward(rewardsTable, questID);
                        questData.questTask = ParseTask(tasksTable, questID);
                        questData.prerequisites = ParsePrerequisites(prerequisitesTable, questID);
                    }

                    quests.Add(questData);
                }

                string json = JsonConvert.SerializeObject(new { quests }, Formatting.Indented);
                File.WriteAllText(jsonPath, json);
                return true;
            }


        }



        // QuestReward 파싱 메서드
        private QuestReward ParseReward(DataTable rewardsTable, int questID)
        {
            QuestReward reward = new QuestReward();

            foreach (DataRow row in rewardsTable.Rows)
            {
                if (int.Parse(row["QuestID"].ToString()) == questID)
                {
                    reward.rewardExp = float.Parse(row["RewardExp"].ToString());
                    reward.rewardMoney = int.Parse(row["RewardMoney"].ToString());

                    reward.questItems = new QuestItem[] { };
                    string[] itemIDs = row["ItemIDs"].ToString().Split(',');
                    string[] itemAmounts = row["ItemAmounts"].ToString().Split(',');

                    for (int i = 0; i < itemIDs.Length; i++)
                    {
                        QuestItem item = new QuestItem
                        {
                            itemID = int.Parse(itemIDs[i]),
                            itemAmount = int.Parse(itemAmounts[i])
                        };
                        reward.questItems[i] = item;
                    }

                    break;
                }
            }

            return reward;
        }

        // QuestTask 파싱 메서드
        private Task ParseTask(DataTable tasksTable, int questID)
        {
            Task task = new Task
            {
                questNeededNPCs = new NPCToTalk[] { },
                questNeededItems = new QuestItem[] { },
                questNeededEnemies = new QuestKill[] { },
            };

            foreach (DataRow row in tasksTable.Rows)
            {
                if (int.Parse(row["QuestID"].ToString()) == questID)
                {
                    // NPC 목록 추가
                    string[] npcIDs = row["NPCIDs"].ToString().Split(',');                    
                    for(int i = 0; i < npcIDs.Length; i++)
                    {
                        task.questNeededNPCs[i] = new NPCToTalk { idOfNPC_NeededToTalk = int.Parse(npcIDs[i]), readyToComplete = false };
                    }

                    // Item 목록 추가
                    string[] itemIDs = row["ItemIDs"].ToString().Split(',');
                    string[] itemAmounts = row["ItemAmounts"].ToString().Split(',');
                    for (int i = 0; i < itemIDs.Length; i++)
                    {
                        task.questNeededItems[i] = new QuestItem
                        {
                            itemID = int.Parse(itemIDs[i]),
                            itemAmount = int.Parse(itemAmounts[i])
                        };
                    }

                    // Enemy 목록 추가
                    string[] enemyIDs = row["EnemyIDs"].ToString().Split(',');
                    string[] enemyAmounts = row["EnemyAmounts"].ToString().Split(',');
                    for (int i = 0; i < enemyIDs.Length; i++)
                    {
                    task.questNeededEnemies[i] = new QuestKill
                        {
                            enemyID = int.Parse(enemyIDs[i]),
                            enemyAmount_ToKill = int.Parse(enemyAmounts[i]),
                            InitialKillAmount_atStart = 0
                        };
                    }

                    break;
                }
            }

            return task;
        }

        // Prerequisites 파싱 메서드
        private Prerequisites ParsePrerequisites(DataTable prerequisitesTable, int questID)
        {
            Prerequisites prerequisites = new Prerequisites();

            foreach (DataRow row in prerequisitesTable.Rows)
            {
                if (int.Parse(row["QuestID"].ToString()) == questID)
                {
                    prerequisites.talkedNPC_id = int.Parse(row["TalkedNPC_ID"].ToString());
                    prerequisites.completedQuestID = int.Parse(row["CompletedQuestID"].ToString());
                    break;
                }
            }

            return prerequisites;
        }
    }

    

}
