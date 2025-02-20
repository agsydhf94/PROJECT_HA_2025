using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(menuName = "Impact System/Surface", fileName = "Surface")]
    public class SurfaceSO : ScriptableObject
    {
        [Serializable]
        public class SurfaceImpactTypeEffect
        {
            public ImpactTypeSO ImpactType;
            public SurfaceEffectSO SurfaceEffect;
        }
        public List<SurfaceImpactTypeEffect> ImpactTypeEffects = new List<SurfaceImpactTypeEffect>();
    }
}
