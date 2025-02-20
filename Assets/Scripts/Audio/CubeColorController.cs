using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CubeColorController : MonoBehaviour
    {
        public Material cubeMaterial;
        public Color gradientStart;
        public Color gradientEnd;
        public Color solidColor;
        public float gradientThreshold = 0.7f;

        void Update()
        {
            // ���̴� ������Ƽ ������Ʈ
            cubeMaterial.SetColor("_GradientColor1", gradientStart);
            cubeMaterial.SetColor("_GradientColor2", gradientEnd);
            cubeMaterial.SetColor("_SolidColor", solidColor);
            cubeMaterial.SetFloat("_Threshold", gradientThreshold);
        }
    }
}
