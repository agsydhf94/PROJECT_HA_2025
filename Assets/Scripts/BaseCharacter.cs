using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    private string name;
    private string description;
    private float strength;
    private float defense;
    private float dexterity;
    private float intelligence;
    private float health;

    public string NAME
    {
        get { return name; }
        set { name = value; }
    }

    public string DESCRIPTION
    {
        get { return description; }
        set { description = value; }
    }

    public float STRENGTH
    {
        get { return strength; }
        set { strength = value; }
    }

    public float DEFENCE
    {
        get { return defense; }
        set { defense = value; }
    }

    public float DEXTERITY
    {
        get { return dexterity; }
        set { dexterity = value; }
    }

    public float INTELLIGENCE
    {
        get { return intelligence; }
        set { intelligence = value; }
    }

    public float HEALTH
    {
        get { return health; }
        set { health = value; }
    }



}
