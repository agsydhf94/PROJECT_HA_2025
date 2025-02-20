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
        private MotionBlur motionBlur; // Motion Blur 효과
        public float montionBlurIntensity = 270f; // 총 쏠 때 Shutter Angle (Motion Blur 강도)
        public float blurDuration = 0.1f; // Blur 유지 시간

        private bool isShooting = false; // 총을 쏘는 상태 확인용

        private void Start()
        {
            // Post Processing Volume에서 Motion Blur 설정 가져오기
            if (volumeProfile.profile.TryGet(out motionBlur))
            {
                motionBlur.active = false; // 시작 시 비활성화
                motionBlur.intensity.value = 0f; // Shutter Angle 초기화
            }
        }

        public void TriggerMotionBlur()
        {
            if (!isShooting) // 이미 Blur가 활성화 중이면 중복 실행 방지
            {
                StartCoroutine(ActivateMotionBlur());
            }
        }

        private IEnumerator ActivateMotionBlur()
        {
            isShooting = true;

            // Motion Blur 활성화
            if (motionBlur != null)
            {
                motionBlur.active = true;
                motionBlur.intensity.value = montionBlurIntensity; // Blur 강도 설정
            }

            // Blur 유지 시간 대기
            yield return new WaitForSeconds(blurDuration);

            // Motion Blur 비활성화
            if (motionBlur != null)
            {
                motionBlur.active = false;  // 완전히 비활성화
                motionBlur.intensity.value = 0f; // Blur 강도를 0으로 설정
            }

            isShooting = false;
        }
    }
}
