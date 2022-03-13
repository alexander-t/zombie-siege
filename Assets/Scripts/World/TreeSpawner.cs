using UnityEngine;
using VLNature;

namespace World
{
    public class TreeSpawner : MonoBehaviour
    {
        public GameObject treePrefab;
        private TerrainSection terrainSection;

        void Awake()
        {
            UnityEngine.Random.InitState(2022);
        }
        void Start()
        {
            terrainSection = World.Instance.GetTerrainSection(0, 0);

            for (int z = 0; z < terrainSection.ZSize; ++z)
            {
                for (int x = 0; x < terrainSection.XSize; ++x)
                {
                    if (!terrainSection.OffLimits(x, z))
                    {
                        if (Random.Range(0, 1.0f) >= 1 - GetTreeProbablity(x, z))
                        {
                            GameObject tree = Instantiate(treePrefab, terrainSection.GetWorldCoords(x, z), Quaternion.identity);
                            TreeGenerator generator = tree.GetComponent<TreeGenerator>();
                            generator.height = Random.Range(5, 8);
                            generator.maxTiers = Random.Range(3, 6);
                            generator.probabilityToBranchOnTrunk = Random.Range(0.5f, 1.0f);
                        }
                    }
                }
            }
        }

        private float GetTreeProbablity(int x, int z)
        {
            const int SquaresNearEdge = 5;
            if (x <= SquaresNearEdge || x >= terrainSection.XSize - SquaresNearEdge || z <= SquaresNearEdge || z >= terrainSection.ZSize - SquaresNearEdge)
            {
                return 0.02f;
            }
            return 0.006f;
        }
    }
}
