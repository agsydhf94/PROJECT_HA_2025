using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class AudioSpectrumVisualizer : MonoBehaviour
    {
        public AudioSource audioSource; // 오디오 소스
        public GameObject cubePrefab; // 큐브 프리팹
        public int numberOfCubes = 64; // 스펙트럼 큐브 수
        public float maxHeight = 10f; // 큐브의 최대 높이
        public Gradient lowHeightGradient; // 0 ~ 0.7 구간의 색상 그래디언트
        public Color highHeightColor; // 0.7 ~ 1 구간의 단색

        private GameObject[] cubes; // 생성된 큐브 배열
        private float[] spectrumData; // 오디오 스펙트럼 데이터

        void Start()
        {
            // 큐브 배열 초기화
            cubes = new GameObject[numberOfCubes];
            spectrumData = new float[numberOfCubes];

            // 큐브 생성
            for (int i = 0; i < numberOfCubes; i++)
            {
                GameObject cube = Instantiate(cubePrefab, new Vector3(i - numberOfCubes / 2, 0, 0), Quaternion.identity);
                cube.transform.SetParent(transform);
                cubes[i] = cube;
            }
        }

        void Update()
        {
            // 오디오 스펙트럼 데이터 가져오기
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);

            // 각 큐브의 스케일과 색상 업데이트
            for (int i = 0; i < numberOfCubes; i++)
            {
                float height = Mathf.Clamp(spectrumData[i] * maxHeight, 0, maxHeight);

                // 큐브 스케일 변경
                Vector3 newScale = cubes[i].transform.localScale;
                newScale.y = height;
                cubes[i].transform.localScale = newScale;

                // 큐브 위치 조정 (스케일에 따른 상단 위치 고정)
                Vector3 newPosition = cubes[i].transform.localPosition;
                newPosition.y = height / 2f;
                cubes[i].transform.localPosition = newPosition;

                // 큐브 색상 처리
                Renderer cubeRenderer = cubes[i].GetComponent<Renderer>();

                if (height / maxHeight <= 0.7f)
                {
                    // 0 ~ 0.7: 그래디언트 색상
                    float normalizedHeight = (height / maxHeight) / 0.7f; // 0 ~ 0.7 범위를 0 ~ 1로 정규화
                    cubeRenderer.material.color = lowHeightGradient.Evaluate(normalizedHeight);
                }
                else
                {
                    // 0.7 ~ 1: 단색
                    cubeRenderer.material.color = highHeightColor;
                }
            }
        }
    }
}
