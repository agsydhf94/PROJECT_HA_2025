using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ShakingUI : MonoBehaviour
    {
        public Vector3 basePosition; // 기본 위치 (화면상 위치)
        public float shakeIntensity = 0.05f; // 흔들림 강도
        public float shakeSpeed = 2f; // 흔들림 속도 조절

        private void Start()
        {
            // UI의 기본 위치를 화면 기준으로 설정 (초기 위치 지정)
            basePosition = transform.position;
        }

        private void LateUpdate()
        {
            // Perlin Noise를 이용한 흔들림 효과
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f
            ) * shakeIntensity;

            // 기본 위치에 흔들림을 더하여 고정된 위치에서 살짝 움직이도록 설정
            transform.position = basePosition + shakeOffset;
        }
    }
}
