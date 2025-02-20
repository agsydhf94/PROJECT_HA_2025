using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStrategy_Manager : Singleton<EquipmentStrategy_Manager>
{
    private IEquipStrategy i_equipStrategy;


    // 장비 전략을 설정하는 메서드
    public void SetEquipStrategy(IEquipStrategy strategy)
    {
        i_equipStrategy = strategy;
    }

    // ItemType에 따라 자동으로 전략 선택
    public void AutoSelectStrategy(ItemType itemType)
    {        

        if(itemType == ItemType.Weapon)
        {
            SetEquipStrategy(new SingleEquipStrategy());
            Debug.Log("단일 장착 전략을 불러오는 아이템 타입");
        }
        else if(itemType == ItemType.shoulder || itemType == ItemType.arm || itemType == ItemType.legs)
        {
            SetEquipStrategy(new DualEquipStrategy());
            Debug.Log("이중 장착 전력을 불러오는 아이템 타입");
        }
    }

    // 장비 장착 메서드
    public void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData)
    {
        if (i_equipStrategy != null)
        {
            i_equipStrategy.Equip(equipmentSO, transformOffsetData);
        }
        else
        {
            Debug.LogWarning("장비 전략이 설정되지 않았습니다.");
        }
    }
}
