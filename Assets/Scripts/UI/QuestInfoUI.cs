using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class QuestInfoUI : UIBase
    {
        public static QuestInfoUI instance;
        public static QuestInfoUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<QuestInfoUI>();
                }
                return instance;
            }
        }

        [Header("Quest Information UI")]
        public GameObject questUIBackground_Panel;
        public GameObject questNamePanel;
        public GameObject questDescriptionPanel;
        public GameObject questTaskPanel;
        public GameObject questRewardPanel;
        public GameObject questRewardPrefabPanel;
        public GameObject questRewardUIPrefab;
        public GameObject questRewardUIPrefab_EXP;
        public GameObject questRewardUIPrefab_Money;
        public Button questAcceptButton;
        public Button questCancelButton;

        public Canvas questInfoUI_Canvas;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

            questInfoUI_Canvas.worldCamera = Camera.main;
            questInfoUI_Canvas.planeDistance = -0.5f;
        }

        public void QuestInfoUI_ON()
        {
            questInfoUI_Canvas.planeDistance = 1.3f;
        }

        public void QuestInfoUI_OFF()
        {
            questInfoUI_Canvas.planeDistance = -0.5f;
        }
    }
}
