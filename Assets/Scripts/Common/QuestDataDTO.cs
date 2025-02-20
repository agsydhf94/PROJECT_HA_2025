using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class QuestDataDTO
    {
        public List<int> finishedQuests = new List<int>();
        public List<ActiveQuestDTO> activeQuests = new List<ActiveQuestDTO>();
        public List<NPCTalkedDTO> npctalked_byPlayer = new List<NPCTalkedDTO>();
    }




    [System.Serializable]
    public class ActiveQuestDTO
    {
        public int questID;
        public string startDate;
        public List<KillProgressDTO> killProgress = new List<KillProgressDTO>();
        public List<NPCProgressDTO> npcProgress = new List<NPCProgressDTO>();
    }

    [System.Serializable]
    public class KillProgressDTO
    {
        public int enemyID;
        public int totalAmount_SoFar;
    }

    [System.Serializable]
    public class NPCProgressDTO
    {
        public int npcID;
        public bool talked;
    }

    [System.Serializable]
    public class NPCTalkedDTO
    {
        public int npcID;
        public int talkedSessionCount;
    }


}
