using ExcelDataReader;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

namespace HA
{
    /*
        NPCDialogueData는 NPC가 사용할 대사와 퀘스트 ID등의 정보를 
        저장하기 위한 데이터 컨테이너 역할을 합니다
        Json 또는 기타 형식의 데이터 파일에서 대사 정보를 로드할 때 사용됩니다
    */

    [System.Serializable]
    public class NPCDialogueData
    {
        public int npcID;
        public List<string> dialogues;
        public bool okay_ToStart;
        public int questID;

    }




    // 배열 형식의 JSON에서 요소를 담기위한 준비
    [System.Serializable]
    public class NPCDialoguesDataContainer
    {
        public List<NPCDialogueData> npcDialogues; // 모든 NPC 대사 데이터를 저장하는 리스트
    }






    /*
        NPCDialogueLoader는 NPC의 대사 및 퀘스트 정보를 저장하며,
        Json 파일에서 데이터를 불러와서 NPC가 대사를 시작할 때 사용합니다

        NPCDialogueLoader가 해당되는 Json 파일을 로드하여
        NPCDialogueData 형식으로 변환합니다
    */
    public class NPCDialogueLoader
    {
        public static NPCDialogueData LoadNPCDialogue(int npcID)
        {
            // JSON 파일을 로드
            TextAsset jsonText = Resources.Load<TextAsset>("DialogueData/DialogueData");


            // JSON 파일을 NPCDialoguesDataContainer로 변환
            NPCDialoguesDataContainer dialoguesData = JsonUtility.FromJson<NPCDialoguesDataContainer>(jsonText.text);


            // 해당 NPC의 ID에 맞는 데이터를 검색
            foreach (NPCDialogueData npcDialogueData in dialoguesData.npcDialogues)
            {
                if (npcDialogueData.npcID == npcID)
                {
                    return npcDialogueData; // 일치하는 NPC 데이터를 반환
                }
            }

            // 해당 ID의 NPC 대사가 없으면 null 반환
            return null;
        }

        public static List<NPCDialogueData> LoadDialoguesFromCSV(string csvPath)
        {
            List<NPCDialogueData> npcDialogues = new List<NPCDialogueData>();

            if (!File.Exists(csvPath))
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
                return npcDialogues;
            }

            using (var stream = File.Open(csvPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    DataTable table = dataSet.Tables[0]; // 첫 번째 시트를 사용한다고 가정

                    for (int i = 1; i < table.Rows.Count; i++) // 첫 번째 행은 헤더로 간주
                    {
                        DataRow row = table.Rows[i];

                        NPCDialogueData dialogueData = new NPCDialogueData
                        {
                            npcID = int.Parse(row["npcID"].ToString()),
                            dialogues = new List<string>(row["dialogues"].ToString().Split('|')),
                            okay_ToStart = bool.Parse(row["okay_ToStart"].ToString()),
                            questID = int.Parse(row["questID"].ToString())
                        };

                        npcDialogues.Add(dialogueData);
                    }
                }
            }

            return npcDialogues;
        }

    }
}