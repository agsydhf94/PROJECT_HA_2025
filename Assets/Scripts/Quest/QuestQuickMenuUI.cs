using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HA
{
    public class QuestQuickMenuUI : UIBase
    {
        public static QuestQuickMenuUI instance;
        public static QuestQuickMenuUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<QuestQuickMenuUI>();
                }
                return instance;
            }
        }



        public int questID;
        public TMP_Text questName;
        public TMP_Text questDescription;
        public TMP_Text questProgress;
        public GameObject questCompletedTextObject;
        public Canvas questQuickMenuCanvas;
        public bool isQuestQuickMenuOn;


        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

        }

        private void Start()
        {
            QuestQuickMenuUI_Initialize();
        }

        public void QuestQuickMenuUI_Initialize()
        {
            QuestManager.QuestProgressUpdated += QuickQuestMenuUI_ProgressText;

            questQuickMenuCanvas.worldCamera = Camera.main;
            questQuickMenuCanvas.planeDistance = -0.5f;
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.M) && !isQuestQuickMenuOn)
            {
                isQuestQuickMenuOn = true;
                questQuickMenuCanvas.planeDistance = 1.3f;
            }
            else if(Input.GetKeyDown(KeyCode.M) && isQuestQuickMenuOn)
            {
                isQuestQuickMenuOn = false;
                questQuickMenuCanvas.planeDistance = -0.5f;
            }
        }

        private void OnDisable()
        {
            QuestManager.QuestProgressUpdated -= QuickQuestMenuUI_ProgressText;
        }

        public void QuickQuestMenuUI_ProgressText(int id, string progressText)
        {
            Debug.Log($"들어왔는데, if 조건문 결과 : {QuestManager.activeQuestButtons[questID].GetComponent<ActiveQuestButton>().quickMenuSelected.activeSelf}");

            if (QuestManager.activeQuestButtons[questID].GetComponent<ActiveQuestButton>().quickMenuSelected.activeSelf)
            {
                questProgress.text = progressText;
                questCompletedTextObject.SetActive(QuestManager.Instance.IsQuestCompleted(id));
            }           
        }

        public void QuickQuestMenuUI_Reset()
        {
            questName.text = "";
            questDescription.text = "";
            questProgress.text = "";
            questCompletedTextObject.SetActive(false);
        }

        
    }
}
