using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HA
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        public void SetMasterVolume(float level)
        {
            audioMixer.SetFloat("MasterVolume", level);
        }
        public void SetFXVolume(float level)
        {
            audioMixer.SetFloat("SoundFX", level);
        }
        public void SetMusicVolume(float level)
        {
            audioMixer.SetFloat("Music", level);
        }
    }
}
