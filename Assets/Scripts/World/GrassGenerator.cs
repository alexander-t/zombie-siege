using UnityEngine;

namespace World
{
    public class GrassGenerator : MonoBehaviour
    {
        [SerializeField] GameObject grassPrefab;
        [SerializeField] GameObject branchPrefab;
        [SerializeField] GameObject grassPatchPrefab;
        [SerializeField] GameObject rockPrefab;


        private TerrainSection terrainSection;

        void Start()
        {
            terrainSection = World.Instance.GetTerrainSection(0, 0);

            for (int z = 0; z < terrainSection.ZSize; ++z)
            {

                for (int x = 0; x < terrainSection.XSize; x++)
                {
                    if (!terrainSection.OffLimits(x, z))
                    {
                        if ((1 - Random.Range(0, 1.0f)) <= 0.075)
                        {
                            GameObject grass = Instantiate(grassPrefab, terrainSection.GetWorldCoords(x, z), Quaternion.identity);
                            grass.transform.Rotate(new Vector3(0, Random.Range(1, 180), 0));
                            grass.transform.Translate(new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)));
                        }
                        else if ((1 - Random.Range(0, 1.0f)) <= 0.01)
                        {
                            GameObject branch = Instantiate(branchPrefab, terrainSection.GetWorldCoords(x, z), Quaternion.identity);
                            branch.transform.Rotate(new Vector3(0, Random.Range(1, 180), 0));
                            float scale = Random.Range(0.3f, 1f);
                            branch.transform.localScale = new Vector3(scale, scale, scale);
                        }
                        else if ((1 - Random.Range(0, 1.0f)) <= 0.01)
                        {
                            Instantiate(rockPrefab, terrainSection.GetWorldCoords(x, z), Quaternion.identity);
                        }
                        else if (Mathf.PerlinNoise(x * 0.7f, z * 0.3f) > 0.70f)
                        {
                            Instantiate(grassPatchPrefab, terrainSection.GetWorldCoords(x, z), Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}