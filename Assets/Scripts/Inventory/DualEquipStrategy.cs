using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualEquipStrategy : MonoBehaviour, IEquipStrategy
{
    public void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData)
    {
        Debug.Log("이중 장착 젼략메서드 진입");

        // InventoryManager에서 이미 프리팹을 불러왔기 때문에, 여기서는 attachPoints에 장착만 하면 됩니다
        // GameObject itemPrefab = InventoryManager.Instance.GetItemPrefab(equipmentSO.itemName);
        GameObject l_EquipmentPrefab = equipmentSO.l_EquipmentPrefab;
        GameObject r_EquipmentPrefab = equipmentSO.r_EquipmentPrefab;

        // 2곳에 프리팹을 장착시켜야 하므로, 2개의 Transform 정보를 취사선택
        Transform l_ParentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.l_ParentTAG).transform;
        Transform r_ParentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.r_ParentTAG).transform;



        // 아이템 프리팹이 제대로 전달되었는지 확인

        if (l_EquipmentPrefab != null && r_EquipmentPrefab != null)
        {
            Debug.Log("이중 장착로직 진입");

            // 각 부위에 장착되어 있던 장비 제거
            GameObject.Destroy(l_ParentTransform.GetChild(0).gameObject);
            GameObject.Destroy(r_ParentTransform.GetChild(0).gameObject);

            // 각 부위에 새로운 장비를 장착
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
            Debug.LogError($"아이템 '{equipmentSO.itemName}'에 해당하는 프리팹을 찾을 수 없습니다.");
        }
        
    }


}

