using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class QuestBookUI : UIBase
    {
        public static QuestBookUI instance;
        public static QuestBookUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<QuestBookUI>();
                }
                return instance;
            }
        }

        // 퀘스트 북 버튼 프리팹에 넣으면(동적으로 처리하면)
        // 씬이 넘어갈 경우 정보가 사라지게 되어 하이어라키 상에서 정적으로 처리해야 함
        [Header("Book Panel - Activated Quest")]
        [SerializeField]
        public Transform bookPanel_Activated_Contents;
        public TMP_Text bookPanel_Activated_QuestName;
        public TMP_Text bookPanel_Activated_QuestDescription;
        public TMP_Text bookPanel_Activated_QuestTask;
        public TMP_Text bookPanel_Activated_QuestProgress;
        public GameObject bookPanel_Activated_QuestReward;
        public Button questCompleteButton;

        [Header("Book Panel - Completed Quest")]
        [SerializeField]
        public Transform bookPanel_Completed_Contents;
        public TMP_Text bookPanel_Completed_QuestName;
        public TMP_Text bookPanel_Completed_QuestDescription;
        public TMP_Text bookPanel_Compledted_QuestTask;
        public TMP_Text bookPanel_Completed_Date;
        public GameObject bookPanel_Completed_QuestReward;

        public QuestData questData_Of_ThisButton;

        // 퀘스트 Reward UI 프리팹을 저장하는 장소
        public List<QuestRewardUI_Item> questRewardUI_Of_ThisButton;

        public Canvas questBookCanvas;
        public GameObject bookPanel_Activated;
        public GameObject bookPanel_Completed;
        public bool isQuestBookOn;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

            questBookCanvas.worldCamera = Camera.main;
            questBookCanvas.planeDistance = -0.5f;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q) && !isQuestBookOn)
            {
                isQuestBookOn = true;
                questBookCanvas.planeDistance = 1.3f;                                

                bookPanel_Activated.SetActive(true);
                bookPanel_Completed.SetActive(false);

            }
            else if(Input.GetKeyDown(KeyCode.N) && isQuestBookOn)
            {
                bookPanel_Activated.SetActive(false);
                bookPanel_Completed.SetActive(true);

            }
            else if(Input.GetKeyDown(KeyCode.B) && isQuestBookOn)
            {
                bookPanel_Activated.SetActive(true);
                bookPanel_Completed.SetActive(false);
            }
            else if(Input.GetKeyDown(KeyCode.Q) && isQuestBookOn)
            {
                isQuestBookOn = false;
                questBookCanvas.planeDistance = -0.5f;
            }
        }
    }
}
