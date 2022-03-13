using Npc;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject[] spawnPoints;
    public float spawnInterval = 10;

    private float spawnTimeElapsed;

    void Start()
    {
        spawnTimeElapsed = 0;
    }

    void Update()
    {
        spawnTimeElapsed += Time.deltaTime;
        if (spawnTimeElapsed >= spawnInterval)
        {
            GameObject zombie = Instantiate(zombiePrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            if (Random.Range(0, 1.0f) >= 0.75)
            {
                zombie.GetComponent<NpcData>().Npc = NpcData.StandardZombieElite;
            }
            else
            {
                zombie.GetComponent<NpcData>().Npc = NpcData.StandardZombie;
            }
            spawnTimeElapsed = 0;
        }
    }
}
