using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(menuName = "Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
    public class PlayAudioEffectSO : ScriptableObject
    {
        public AudioSource AudioSourcePrefab;
        public List<AudioClip> AudioClips = new List<AudioClip>();

        [Tooltip("Values are clamped to 0-1")]
        public Vector2 VolumeRange = new Vector2(0, 1);
    }
}
