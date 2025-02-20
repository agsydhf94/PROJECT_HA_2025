using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{

    public class MemoryPool : MonoBehaviour
    {
        public class PoolItem
        {
            public bool isActive;  // gameObject�� Ȱ��ȭ/��Ȱ��ȭ ����
            public GameObject gameObject;
        }

        private int increaseCount = 5;  // ������Ʈ�� ������ �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
        private int maxCount;           // ���� ����Ʈ�� ��ϵǾ��ִ� ������Ʈ ����
        private int activeCount;        // ���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ) ������Ʈ ����

        private GameObject poolObject;       // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
        private List<PoolItem> poolItemList; // �����Ǵ� ��� ������Ʈ�� ����

        public int MaxCount => maxCount;  // �ܺο��� ���� ����Ʈ�� ��ϵǾ��ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
        public int ActiveCount => activeCount; // �ܺο��� ���� Ȱ��ȭ �Ǿ��ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

        public MemoryPool(GameObject poolObject)
        {
            maxCount = 0;
            activeCount = 0;
            this.poolObject = poolObject;

            poolItemList = new List<PoolItem>();
        }

        // increaseCount ������ ������Ʈ�� ����
        
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

            // ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
            // ��� ������Ʈ�� Ȱ��ȭ �����̸� ���ο� ������Ʈ �ʿ�
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