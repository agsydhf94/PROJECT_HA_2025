using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [RequireComponent (typeof (AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        public AudioSource audioSource;
        public static float[] samples_ = new float[64];
        public static float[] processedSamples_ = new float[64]; // 정규화된 데이터를 저장

        private float maxSampleValue = 0.01f; // 정규화에 필요한 최대값

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            GetSpectrumAudioSource();
            NormalizeSpectrumData();
        }

        private void GetSpectrumAudioSource()
        {
            // 오디오 데이터를 가져옵니다.
            audioSource.GetSpectrumData(samples_, 0, FFTWindow.Blackman);
        }

        private void NormalizeSpectrumData()
        {
            // 전체 샘플 중 최대값을 찾아 정규화
            for (int i = 0; i < samples_.Length; i++)
            {
                if (samples_[i] > maxSampleValue)
                {
                    maxSampleValue = samples_[i];
                }
            }

            // 정규화 후 저장
            for (int i = 0; i < samples_.Length; i++)
            {
                processedSamples_[i] = samples_[i] / maxSampleValue; // 0~1로 정규화
                processedSamples_[i] = Mathf.Pow(processedSamples_[i], 0.5f); // 루트를 사용하여 작은 값 부스팅
            }
        }
    }
}
