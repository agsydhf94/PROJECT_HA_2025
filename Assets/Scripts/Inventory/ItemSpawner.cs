using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    public Transform playerTransform;

    public float itemSpawnMaxDistance = 5f;

    public float maxItemSpawnTerm = 6f;
    public float minItemSpawnTerm = 2.2f;
    private float itemSpawnTerm;

    private float lastItemSpawnTime;

    private void Start()
    {
        itemSpawnTerm = Random.Range(minItemSpawnTerm, maxItemSpawnTerm);
        lastItemSpawnTime = 0;
    }

    private void Update()
    {
        if(Time.time > lastItemSpawnTime + itemSpawnTerm && playerTransform != null)
        {
            lastItemSpawnTime = Time.time;
            itemSpawnTerm = Random.Range(minItemSpawnTerm, maxItemSpawnTerm);
            ItemSpawn();
        }
    }

    public void ItemSpawn()
    {
        Vector3 spawnPosition = RandomPosition_NavMesh(playerTransform.position, maxItemSpawnTerm);
        
        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = Instantiate(selectedItem, spawnPosition, Quaternion.identity);

        Destroy(item, 5.5f);
    }

    public Vector3 RandomPosition_NavMesh(Vector3 center, float radius)
    {
        Vector3 randomPosition = Random.insideUnitSphere * radius + center;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }

}
