using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HA
{
    public class MotionBlurController : Singleton<MotionBlurController>
    {
        public Volume volumeProfile; // Post Processing Volume
        private MotionBlur motionBlur; // Motion Blur ȿ��
        public float montionBlurIntensity = 270f; // �� �� �� Shutter Angle (Motion Blur ����)
        public float blurDuration = 0.1f; // Blur ���� �ð�

        private bool isShooting = false; // ���� ��� ���� Ȯ�ο�

        private void Start()
        {
            // Post Processing Volume���� Motion Blur ���� ��������
            if (volumeProfile.profile.TryGet(out motionBlur))
            {
                motionBlur.active = false; // ���� �� ��Ȱ��ȭ
                motionBlur.intensity.value = 0f; // Shutter Angle �ʱ�ȭ
            }
        }

        public void TriggerMotionBlur()
        {
            if (!isShooting) // �̹� Blur�� Ȱ��ȭ ���̸� �ߺ� ���� ����
            {
                StartCoroutine(ActivateMotionBlur());
            }
        }

        private IEnumerator ActivateMotionBlur()
        {
            isShooting = true;

            // Motion Blur Ȱ��ȭ
            if (motionBlur != null)
            {
                motionBlur.active = true;
                motionBlur.intensity.value = montionBlurIntensity; // Blur ���� ����
            }

            // Blur ���� �ð� ���
            yield return new WaitForSeconds(blurDuration);

            // Motion Blur ��Ȱ��ȭ
            if (motionBlur != null)
            {
                motionBlur.active = false;  // ������ ��Ȱ��ȭ
                motionBlur.intensity.value = 0f; // Blur ������ 0���� ����
            }

            isShooting = false;
        }
    }
}
