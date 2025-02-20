using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CheckPointManager : MonoBehaviour
    {
        public static CheckPointManager Instance { get; private set; }
        public List<int> unlockedCheckPoints = new List<int>();



        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void SaveCheckPoint(int checkpointID)
        {
            if (!unlockedCheckPoints.Contains(checkpointID))
            {
                unlockedCheckPoints.Add(checkpointID);                
            }
        }



    }
}
