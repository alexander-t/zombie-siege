using Firing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class ResourceSpawner : MonoBehaviour
{
    private GenericObjectPool woodPool;

    private float spwanInterval = 1;
    private float timeElapsed;


    private TerrainSection terrainSection;

    void Start()
    {
        timeElapsed = 0;
        terrainSection = World.World.Instance.GetTerrainSection(0, 0);
        woodPool = GetComponent<GenericObjectPool>();
    }


    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= spwanInterval)
        {
            GameObject log = woodPool.FindUnusedObject();
            if (log != null)
            {
                Vector3 spawnPoint = terrainSection.GetWorldCoords(Random.Range(5, terrainSection.XSize - 5), Random.Range(5, terrainSection.ZSize - 5));

                log.transform.position = spawnPoint;
                log.transform.Rotate(Vector3.up, Random.Range(0, 360));
                log.SetActive(true);
            }
            timeElapsed = 0;
        }
    }
}
