using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HA
{
    public class SkillTreeUI : UIBase
    {
        public Canvas skillTreeCanvasUI_Canvas;
        private bool isSkillTreeUIOn;
        public TMP_Text skillDescription;

        [Header("AsunaSkillUI")]
        public GameObject asuna_SkillTreeUI;
        public SkillSlot[] asuna_SkillSlotUI;

        [Header("KunoichiSkillUI")]
        public GameObject kunoichi_SkillTreeUI;
        public SkillSlot[] kunoichi_SkillSlotUI;


        private void Awake()
        {
            skillTreeCanvasUI_Canvas.worldCamera = Camera.main;
            skillTreeCanvasUI_Canvas.planeDistance = -0.5f;

            SkillManager.Instance.selected_SkillDescription = skillDescription;

            SkillManager.Instance.asunaSkillTree_UI = asuna_SkillTreeUI;
            SkillManager.Instance.asunaSkillSlots_UI = asuna_SkillSlotUI;

            SkillManager.Instance.kunoichiSkillSlots_UI = kunoichi_SkillSlotUI;
            SkillManager.Instance.kunoichiSkillTree_UI = kunoichi_SkillTreeUI;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.C) && !isSkillTreeUIOn)
            {
                isSkillTreeUIOn = true;
                SkillTreeUIOn();
            }
            else if(Input.GetKeyDown(KeyCode.C) && isSkillTreeUIOn)
            {
                isSkillTreeUIOn = false;
                SkillTreeUIOff();
            }


        }

        private void SkillTreeUIOn()
        {
            skillTreeCanvasUI_Canvas.planeDistance = 1.3f;
        }

        private void SkillTreeUIOff()
        {
            skillTreeCanvasUI_Canvas.planeDistance = -0.5f;
        }
    }
}
