using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private float fadeSpeed = 4.0f;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            StartCoroutine("OnFadeEffect");
        }

        private void OnDisable()
        {
            StopCoroutine("OnFadeEffect");
        }

        private IEnumerator OnFadeEffect()
        {
            while (true)
            {
                Color color = meshRenderer.material.color;
                color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
                meshRenderer.material.color = color;
            }
        }



    }
}

