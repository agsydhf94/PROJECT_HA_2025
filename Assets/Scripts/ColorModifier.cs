using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorModifier : MonoBehaviour
{
    [Header("Hair Color")]
    public SkinnedMeshRenderer hair;
    public Slider hair_R;
    public Slider hair_G;
    public Slider hair_B;

    [Header("Body Color")]
    public SkinnedMeshRenderer body;
    public Slider body_R;
    public Slider body_G;
    public Slider body_B;

    public void HairColorEdit()
    {
        Color color = hair.material.color;
        color.r = hair_R.value;
        color.g = hair_G.value;
        color.b = hair_B.value;
        hair.material.color = color;
        hair.material.SetColor("_EmissionColor", color);
    }

    public void BodyColorEdit()
    {
        Color color = body.material.color;
        color.r = body_R.value;
        color.g = body_G.value;
        color.b = body_B.value;
        body.material.color = color;
        body.material.SetColor("_EmissionColor", color);
    }
}
