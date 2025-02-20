using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(menuName = "Impact System/Surface Effect", fileName = "SurfaceEffect")]
    public class SurfaceEffectSO : ScriptableObject
    {
        public List<SpawnObjectEffectSO> SpawnObjectEffects = new List<SpawnObjectEffectSO>();
        public List<PlayAudioEffectSO> PlayAudioEffects = new List<PlayAudioEffectSO>();
    }
}
