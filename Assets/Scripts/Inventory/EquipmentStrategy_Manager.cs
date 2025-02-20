using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStrategy_Manager : Singleton<EquipmentStrategy_Manager>
{
    private IEquipStrategy i_equipStrategy;


    // ��� ������ �����ϴ� �޼���
    public void SetEquipStrategy(IEquipStrategy strategy)
    {
        i_equipStrategy = strategy;
    }

    // ItemType�� ���� �ڵ����� ���� ����
    public void AutoSelectStrategy(ItemType itemType)
    {        

        if(itemType == ItemType.Weapon)
        {
            SetEquipStrategy(new SingleEquipStrategy());
            Debug.Log("���� ���� ������ �ҷ����� ������ Ÿ��");
        }
        else if(itemType == ItemType.shoulder || itemType == ItemType.arm || itemType == ItemType.legs)
        {
            SetEquipStrategy(new DualEquipStrategy());
            Debug.Log("���� ���� ������ �ҷ����� ������ Ÿ��");
        }
    }

    // ��� ���� �޼���
    public void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData)
    {
        if (i_equipStrategy != null)
        {
            i_equipStrategy.Equip(equipmentSO, transformOffsetData);
        }
        else
        {
            Debug.LogWarning("��� ������ �������� �ʾҽ��ϴ�.");
        }
    }
}
