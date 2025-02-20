using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseItem : MonoBehaviour
{
    [SerializeField]
    private string name;  // ������ �̸�

    [SerializeField]
    private string description;  // ������ ����

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
