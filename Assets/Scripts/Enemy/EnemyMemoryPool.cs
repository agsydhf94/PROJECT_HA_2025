using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


namespace HA
{
    public class EnemyMemoryPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemySpawnPointPrefab;
        [SerializeField]
        private GameObject enemyPrefab;
        [SerializeField]
        private float enemySpawnTime = 1.0f;  // 적 생성 주기
        [SerializeField]
        private float enemySpawnLatency = 1.0f;  // 타일 생성 후 적이 등장하기까지 걸리는 시간

        private MemoryPool spawnPointMemoryPool;
        private MemoryPool enemyMemoryPool;

        private int numberOfEnemiesSpawnedAtOnce = 1;
        private Vector2Int mapSize = new Vector2Int(100, 100);

        private void Awake()
        {
            spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        }
    }
}
