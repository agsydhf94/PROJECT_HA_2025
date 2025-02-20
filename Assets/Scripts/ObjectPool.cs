using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        // 오브젝트 풀 딕셔너리로 여러 종류의 오브젝트 풀을 관리
        public Dictionary<string, Queue<Component>> objectPools = new Dictionary<string, Queue<Component>>();

        // 오브젝트 풀 생성 및 초기화
        public void CreatePool<T>(string poolKey, T prefab, int initialSize) where T : Component
        {
            if (!objectPools.ContainsKey(poolKey))
            {
                objectPools[poolKey] = new Queue<Component>();

                for (int i = 0; i < initialSize; i++)
                {
                    T newObject = Instantiate(prefab);
                    newObject.gameObject.SetActive(false);
                    objectPools[poolKey].Enqueue(newObject);
                }
            }
        }

        // 오브젝트 풀에서 꺼내오기
        public T GetFromPool<T>(string poolKey) where T : Component
        {
            if (objectPools.ContainsKey(poolKey) && objectPools[poolKey].Count > 0)
            {
                T objectToReuse = objectPools[poolKey].Dequeue() as T;
                objectToReuse.gameObject.SetActive(true);
                return objectToReuse;
            }
            else
            {
                // 풀에 오브젝트가 없으면 null 반환 (필요시 새로운 오브젝트를 생성하는 로직 추가 가능)
                return null;
            }
        }

        // 오브젝트를 풀에 반환
        public void ReturnToPool<T>(string poolKey, T objectToReturn) where T : Component
        {
            if (objectPools.ContainsKey(poolKey))
            {
                objectToReturn.gameObject.SetActive(false);
                objectPools[poolKey].Enqueue(objectToReturn);
            }
        }
    }
}
