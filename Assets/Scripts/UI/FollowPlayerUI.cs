using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class FollowPlayerUI : MonoBehaviour
    {
        public Transform player; // 플레이어의 Transform
        public Vector3 offset = new Vector3(0, 2, 0); // 플레이어에 상대적인 UI의 위치 오프셋
        public float shakeIntensity = 0.05f; // UI의 흔들림 강도
        public float shakeSpeed = 2f; // 흔들림 속도 조절

        private Vector3 initialPosition;

        private void Start()
        {
            // UI의 초기 위치를 설정합니다.
            initialPosition = transform.position;
        }

        private void Update()
        {
            player = GameManager.Instance.currentActiveCharacter.transform;
        }

        private void LateUpdate()
        {
            if (player != null)
            {
                // 플레이어의 위치에 offset을 더해 UI의 기본 위치를 설정합니다.
                Vector3 targetPosition = player.position + offset;

                Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f // Z 축 흔들림은 제거
            ) * shakeIntensity;

                // UI의 위치를 업데이트합니다.
                transform.position = targetPosition + shakeOffset;

                // UI가 항상 카메라를 향하도록 회전합니다.
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0); // UI가 카메라를 정면으로 바라보도록 회전
            }
        }
    }
}
