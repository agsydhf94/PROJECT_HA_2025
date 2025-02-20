using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataBase", menuName = "HA/GameDataBase", order = 1)]
public class TurretData : ScriptableObject
{
    public int turretID;
    public float range;
    public float damage;
    public float fireRate;
}
