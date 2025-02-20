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

        // ����Ʈ �� ��ư �����տ� ������(�������� ó���ϸ�)
        // ���� �Ѿ ��� ������ ������� �Ǿ� ���̾��Ű �󿡼� �������� ó���ؾ� ��
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

        // ����Ʈ Reward UI �������� �����ϴ� ���
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
