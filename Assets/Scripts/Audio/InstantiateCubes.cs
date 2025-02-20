using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class InstantiateCubes : MonoBehaviour
    {
        public GameObject sampleCubePrefab;
        public GameObject[] sampleCube_ = new GameObject[64];
        public float maxScale = 10f;
        public float spacing = 1.2f;

        private void Start()
        {
            for (int i = 0; i < sampleCube_.Length; i++)
            {
                // 큐브 생성 및 위치 지정
                var instanceSampleCube = Instantiate(sampleCubePrefab);
                instanceSampleCube.transform.position = new Vector3(i * spacing, 0, 0); // X축으로 나란히 배치
                instanceSampleCube.transform.parent = transform; // 이 스크립트가 붙은 오브젝트의 자식으로 설정
                instanceSampleCube.name = "SampleCube" + i;
                sampleCube_[i] = instanceSampleCube;
            }
        }

        private void Update()
        {
            for (int i = 0; i < sampleCube_.Length; i++)
            {
                if (sampleCube_[i] != null)
                {
                    // processedSamples_ 데이터 사용
                    float scaleY = (AudioPeer.processedSamples_[i] * maxScale) + 1; // 최소 높이 보정
                    sampleCube_[i].transform.localScale = new Vector3(1, scaleY, 1);
                }
            }
        }


    }
}
