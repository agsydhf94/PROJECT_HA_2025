using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipStrategy
{
    void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData);
}
