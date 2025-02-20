using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class FollowPlayerUI : MonoBehaviour
    {
        public Transform player; // �÷��̾��� Transform
        public Vector3 offset = new Vector3(0, 2, 0); // �÷��̾ ������� UI�� ��ġ ������
        public float shakeIntensity = 0.05f; // UI�� ��鸲 ����
        public float shakeSpeed = 2f; // ��鸲 �ӵ� ����

        private Vector3 initialPosition;

        private void Start()
        {
            // UI�� �ʱ� ��ġ�� �����մϴ�.
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
                // �÷��̾��� ��ġ�� offset�� ���� UI�� �⺻ ��ġ�� �����մϴ�.
                Vector3 targetPosition = player.position + offset;

                Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f // Z �� ��鸲�� ����
            ) * shakeIntensity;

                // UI�� ��ġ�� ������Ʈ�մϴ�.
                transform.position = targetPosition + shakeOffset;

                // UI�� �׻� ī�޶� ���ϵ��� ȸ���մϴ�.
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0); // UI�� ī�޶� �������� �ٶ󺸵��� ȸ��
            }
        }
    }
}
