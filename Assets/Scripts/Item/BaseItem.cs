using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseItem : MonoBehaviour
{
    [SerializeField]
    private string name;  // 아이템 이름

    [SerializeField]
    private string description;  // 아이템 설명

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }
}
