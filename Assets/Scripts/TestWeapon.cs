using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class TestWeapon : MonoBehaviour
    {
        private int coroutineCount = 0;

        public IEnumerator SpawnTrail(TrailRenderer trailObject, RaycastHit hit)
        {
            float timer = 0f;
            Vector3 startPosition = trailObject.transform.position;
            Debug.Log(startPosition);

            while (timer <= 1f)
            {
                coroutineCount++;
                Debug.Log($"�ڷ�ƾ while ��ȸ Ƚ��(��) : {coroutineCount}");

                //Debug.Log($"trailObject.time: {trailObject.time}, Time.deltaTime: {Time.deltaTime}");
                yield return new WaitForSecondsRealtime(0.1f);

                Debug.Log($"�ڷ�ƾ while ��ȸ Ƚ��(��) : {coroutineCount}");

                trailObject.transform.position = Vector3.Lerp(startPosition, hit.point, timer);
                timer += Time.deltaTime / trailObject.time;
                //Debug.Log($"trailObject.time: {trailObject.time}, Time.deltaTime: {Time.deltaTime}");
                Debug.Log($"Ÿ�̸� : {timer}");
            }

            
        }


    }
}
