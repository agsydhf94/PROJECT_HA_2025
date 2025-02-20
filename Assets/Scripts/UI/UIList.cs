using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HA
{
    // UI를 넣으려면 각 구역 사이에 넣기
    // 시작과 끝 위치는 참고 지점용도로 사용
    public enum UIList
    {
        PANEL_START, // panel 오브젝트 시작 위치

        LoadingUI,

        TitleUI,

        InteractionUI,

        QuestBookUI,

        QuestInfoUI,

        QuestQuickMenuUI,

        SkillTreeUI,

        InventoryUI,

        WeaponQuickMenuUI,

        PauseMenuUI,

        CheckPopUpUI,

        Prototype_UI_A,

        PANEL_END, // panel 오브젝트 끝



        POPUP_START, // popup 오브젝트 시작 위치

        

        POPUP_END, // popup 오브젝트 끝 
    }
}
