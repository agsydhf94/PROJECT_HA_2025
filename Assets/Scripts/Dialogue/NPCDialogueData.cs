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
        NPCDialogueData�� NPC�� ����� ���� ����Ʈ ID���� ������ 
        �����ϱ� ���� ������ �����̳� ������ �մϴ�
        Json �Ǵ� ��Ÿ ������ ������ ���Ͽ��� ��� ������ �ε��� �� ���˴ϴ�
    */

    [System.Serializable]
    public class NPCDialogueData
    {
        public int npcID;
        public List<string> dialogues;
        public bool okay_ToStart;
        public int questID;

    }




    // �迭 ������ JSON���� ��Ҹ� ������� �غ�
    [System.Serializable]
    public class NPCDialoguesDataContainer
    {
        public List<NPCDialogueData> npcDialogues; // ��� NPC ��� �����͸� �����ϴ� ����Ʈ
    }






    /*
        NPCDialogueLoader�� NPC�� ��� �� ����Ʈ ������ �����ϸ�,
        Json ���Ͽ��� �����͸� �ҷ��ͼ� NPC�� ��縦 ������ �� ����մϴ�

        NPCDialogueLoader�� �ش�Ǵ� Json ������ �ε��Ͽ�
        NPCDialogueData �������� ��ȯ�մϴ�
    */
    public class NPCDialogueLoader
    {
        public static NPCDialogueData LoadNPCDialogue(int npcID)
        {
            // JSON ������ �ε�
            TextAsset jsonText = Resources.Load<TextAsset>("DialogueData/DialogueData");


            // JSON ������ NPCDialoguesDataContainer�� ��ȯ
            NPCDialoguesDataContainer dialoguesData = JsonUtility.FromJson<NPCDialoguesDataContainer>(jsonText.text);


            // �ش� NPC�� ID�� �´� �����͸� �˻�
            foreach (NPCDialogueData npcDialogueData in dialoguesData.npcDialogues)
            {
                if (npcDialogueData.npcID == npcID)
                {
                    return npcDialogueData; // ��ġ�ϴ� NPC �����͸� ��ȯ
                }
            }

            // �ش� ID�� NPC ��簡 ������ null ��ȯ
            return null;
        }

        public static List<NPCDialogueData> LoadDialoguesFromCSV(string csvPath)
        {
            List<NPCDialogueData> npcDialogues = new List<NPCDialogueData>();

            if (!File.Exists(csvPath))
            {
                Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + csvPath);
                return npcDialogues;
            }

            using (var stream = File.Open(csvPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    DataTable table = dataSet.Tables[0]; // ù ��° ��Ʈ�� ����Ѵٰ� ����

                    for (int i = 1; i < table.Rows.Count; i++) // ù ��° ���� ����� ����
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