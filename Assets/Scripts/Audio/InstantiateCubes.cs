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
                // ť�� ���� �� ��ġ ����
                var instanceSampleCube = Instantiate(sampleCubePrefab);
                instanceSampleCube.transform.position = new Vector3(i * spacing, 0, 0); // X������ ������ ��ġ
                instanceSampleCube.transform.parent = transform; // �� ��ũ��Ʈ�� ���� ������Ʈ�� �ڽ����� ����
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
                    // processedSamples_ ������ ���
                    float scaleY = (AudioPeer.processedSamples_[i] * maxScale) + 1; // �ּ� ���� ����
                    sampleCube_[i].transform.localScale = new Vector3(1, scaleY, 1);
                }
            }
        }


    }
}
