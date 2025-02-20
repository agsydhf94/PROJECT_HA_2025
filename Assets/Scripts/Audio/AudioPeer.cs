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
        public static float[] processedSamples_ = new float[64]; // ����ȭ�� �����͸� ����

        private float maxSampleValue = 0.01f; // ����ȭ�� �ʿ��� �ִ밪

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
            // ����� �����͸� �����ɴϴ�.
            audioSource.GetSpectrumData(samples_, 0, FFTWindow.Blackman);
        }

        private void NormalizeSpectrumData()
        {
            // ��ü ���� �� �ִ밪�� ã�� ����ȭ
            for (int i = 0; i < samples_.Length; i++)
            {
                if (samples_[i] > maxSampleValue)
                {
                    maxSampleValue = samples_[i];
                }
            }

            // ����ȭ �� ����
            for (int i = 0; i < samples_.Length; i++)
            {
                processedSamples_[i] = samples_[i] / maxSampleValue; // 0~1�� ����ȭ
                processedSamples_[i] = Mathf.Pow(processedSamples_[i], 0.5f); // ��Ʈ�� ����Ͽ� ���� �� �ν���
            }
        }
    }
}
