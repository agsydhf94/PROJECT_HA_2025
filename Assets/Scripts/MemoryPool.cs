using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{

    public class MemoryPool : MonoBehaviour
    {
        public class PoolItem
        {
            public bool isActive;  // gameObject의 활성화/비활성화 정보
            public GameObject gameObject;
        }

        private int increaseCount = 5;  // 오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수
        private int maxCount;           // 현재 리스트에 등록되어있는 오브젝트 개수
        private int activeCount;        // 현재 게임에 사용되고 있는(활성화) 오브젝트 개수

        private GameObject poolObject;       // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
        private List<PoolItem> poolItemList; // 관리되는 모든 오브젝트를 저장

        public int MaxCount => maxCount;  // 외부에서 현재 리스트에 등록되어있는 오브젝트 개수 확인을 위한 프로퍼티
        public int ActiveCount => activeCount; // 외부에서 현재 활성화 되어있는 오브젝트 개수 확인을 위한 프로퍼티

        public MemoryPool(GameObject poolObject)
        {
            maxCount = 0;
            activeCount = 0;
            this.poolObject = poolObject;

            poolItemList = new List<PoolItem>();
        }

        // increaseCount 단위로 오브젝트를 생성
        
        public void InstantiateObjects()
        {
            maxCount += increaseCount;

            for(int i = 0; i < increaseCount; i++)
            {
                PoolItem poolItem = new PoolItem();

                poolItem.isActive = false;
                poolItem.gameObject = GameObject.Instantiate(poolObject);
                poolItem.gameObject.SetActive(false);

                poolItemList.Add(poolItem);
            }
        }

        public GameObject ActivePoolItem()
        {
            if(poolItemList == null) return null;

            // 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
            // 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
            if(maxCount == activeCount)
            {
                InstantiateObjects();
            }

            int count = poolItemList.Count;
            for(int i = 0; i < count; i++)
            {
                PoolItem poolItem = poolItemList[i];

                if (poolItem.isActive == false)
                {
                    activeCount++;

                    poolItem.isActive = true;
                    poolItem.gameObject.SetActive(true);

                    return poolItem.gameObject;
                }
            }

            return null;
        }

        public void DeactiveAllPoolItems()
        {
            if(poolItemList == null) return;

            int count = poolItemList.Count;
            for(int i = 0; i < count; ++i)
            {
                PoolItem poolItem = poolItemList[i];

                if (poolItem.gameObject != null && poolItem.isActive == true)
                {
                    poolItem.isActive = false;
                    poolItem.gameObject.SetActive(false);
                }
            }
            activeCount = 0;
        }

    }
}