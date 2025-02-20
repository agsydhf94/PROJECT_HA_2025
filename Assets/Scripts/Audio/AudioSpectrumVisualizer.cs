using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class AudioSpectrumVisualizer : MonoBehaviour
    {
        public AudioSource audioSource; // ����� �ҽ�
        public GameObject cubePrefab; // ť�� ������
        public int numberOfCubes = 64; // ����Ʈ�� ť�� ��
        public float maxHeight = 10f; // ť���� �ִ� ����
        public Gradient lowHeightGradient; // 0 ~ 0.7 ������ ���� �׷����Ʈ
        public Color highHeightColor; // 0.7 ~ 1 ������ �ܻ�

        private GameObject[] cubes; // ������ ť�� �迭
        private float[] spectrumData; // ����� ����Ʈ�� ������

        void Start()
        {
            // ť�� �迭 �ʱ�ȭ
            cubes = new GameObject[numberOfCubes];
            spectrumData = new float[numberOfCubes];

            // ť�� ����
            for (int i = 0; i < numberOfCubes; i++)
            {
                GameObject cube = Instantiate(cubePrefab, new Vector3(i - numberOfCubes / 2, 0, 0), Quaternion.identity);
                cube.transform.SetParent(transform);
                cubes[i] = cube;
            }
        }

        void Update()
        {
            // ����� ����Ʈ�� ������ ��������
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);

            // �� ť���� �����ϰ� ���� ������Ʈ
            for (int i = 0; i < numberOfCubes; i++)
            {
                float height = Mathf.Clamp(spectrumData[i] * maxHeight, 0, maxHeight);

                // ť�� ������ ����
                Vector3 newScale = cubes[i].transform.localScale;
                newScale.y = height;
                cubes[i].transform.localScale = newScale;

                // ť�� ��ġ ���� (�����Ͽ� ���� ��� ��ġ ����)
                Vector3 newPosition = cubes[i].transform.localPosition;
                newPosition.y = height / 2f;
                cubes[i].transform.localPosition = newPosition;

                // ť�� ���� ó��
                Renderer cubeRenderer = cubes[i].GetComponent<Renderer>();

                if (height / maxHeight <= 0.7f)
                {
                    // 0 ~ 0.7: �׷����Ʈ ����
                    float normalizedHeight = (height / maxHeight) / 0.7f; // 0 ~ 0.7 ������ 0 ~ 1�� ����ȭ
                    cubeRenderer.material.color = lowHeightGradient.Evaluate(normalizedHeight);
                }
                else
                {
                    // 0.7 ~ 1: �ܻ�
                    cubeRenderer.material.color = highHeightColor;
                }
            }
        }
    }
}
