using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualEquipStrategy : MonoBehaviour, IEquipStrategy
{
    public void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData)
    {
        Debug.Log("���� ���� �����޼��� ����");

        // InventoryManager���� �̹� �������� �ҷ��Ա� ������, ���⼭�� attachPoints�� ������ �ϸ� �˴ϴ�
        // GameObject itemPrefab = InventoryManager.Instance.GetItemPrefab(equipmentSO.itemName);
        GameObject l_EquipmentPrefab = equipmentSO.l_EquipmentPrefab;
        GameObject r_EquipmentPrefab = equipmentSO.r_EquipmentPrefab;

        // 2���� �������� �������Ѿ� �ϹǷ�, 2���� Transform ������ ��缱��
        Transform l_ParentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.l_ParentTAG).transform;
        Transform r_ParentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.r_ParentTAG).transform;



        // ������ �������� ����� ���޵Ǿ����� Ȯ��

        if (l_EquipmentPrefab != null && r_EquipmentPrefab != null)
        {
            Debug.Log("���� �������� ����");

            // �� ������ �����Ǿ� �ִ� ��� ����
            GameObject.Destroy(l_ParentTransform.GetChild(0).gameObject);
            GameObject.Destroy(r_ParentTransform.GetChild(0).gameObject);

            // �� ������ ���ο� ��� ����
            var l_NewEquipment = Instantiate(l_EquipmentPrefab, l_ParentTransform);
            var r_NewEquipment = Instantiate(r_EquipmentPrefab, r_ParentTransform);

            l_NewEquipment.transform.localPosition = transformOffsetData.l_PositionOffset;
            l_NewEquipment.transform.localRotation = Quaternion.Euler(transformOffsetData.l_RotationOffset);
            l_NewEquipment.transform.localScale = transformOffsetData.l_ScaleOffset;

            r_NewEquipment.transform.localPosition = transformOffsetData.r_PositionOffset;
            r_NewEquipment.transform.localRotation = Quaternion.Euler(transformOffsetData.r_RotationOffset);
            r_NewEquipment.transform.localScale = transformOffsetData.r_ScaleOffset;
        }
        else
        {
            Debug.LogError($"������ '{equipmentSO.itemName}'�� �ش��ϴ� �������� ã�� �� �����ϴ�.");
        }
        
    }


}

