using HA;
using UnityEngine;

namespace HA
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioMaster : MonoBehaviour
    {
        private AudioSource _audioSource;
        private string _selectedDevice;
        public static float[] Samples;
        public static float LerpTime;

        [Header("Visuals")]
        [SerializeField] private int audioSamples;
        [SerializeField, Range(0f, 20f)] private float lerpAmount;
        [Header("Audio")]
        [SerializeField] private bool useMicrophone;
        [SerializeField] private float microphoneVolume;
        [SerializeField] private float microphoneScale;
        [SerializeField] private AudioClip audioClip;

        private void Awake()
        {
            //setting necessary values
            Samples = new float[audioSamples];
            LerpTime = lerpAmount;
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (useMicrophone) GetMicrophoneInput();
            else
            {
                PlayAudioClip();
                _audioSource.clip = audioClip;
            }
        }

        private void PlayAudioClip()
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        private void GetMicrophoneInput()
        {
            if (Microphone.devices.Length > 0)
            {
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
                _audioSource.volume = microphoneVolume;
                gameObject.GetComponent<AudioVisualizer>().maxScale = microphoneScale;
                _audioSource.Play();
            }
            else
            {
                useMicrophone = false;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            GetSpectrumData();
        }

        private void GetSpectrumData()
        {
            _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
        }


    }
}
