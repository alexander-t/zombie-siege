using UnityEngine;

namespace World
{
    public class OverlayMeshGenerator : MonoBehaviour
    {
        public Material[] materials;
        private TerrainSection terrainSection;

        private static readonly int[] TextureSizes = new int[] { 1, 2, 4 };

        void Start()
        {
            terrainSection = World.Instance.GetTerrainSection(0, 0);

            for (int z = 0; z < terrainSection.ZSize - TextureSizes[TextureSizes.Length - 1]; z += 2)
            {

                for (int x = 0; x < terrainSection.XSize - TextureSizes[TextureSizes.Length - 1]; x += 2)
                {
                    if ((1 - Random.Range(0, 1.0f)) <= 0.1)
                    {
                        CreateTerrainOverlay(x, z, TextureSizes[Random.Range(0, TextureSizes.Length)]);
                    }
                }
            }
        }

        void CreateTerrainOverlay(int sx, int sz, int squaresPerTexture = 1)
        {
            float textureStep = 1.0f / squaresPerTexture;
            int xSize = squaresPerTexture;
            int zSize = squaresPerTexture;

            Vector3[] vertices = new Vector3[xSize * zSize * TerrainSection.VerticesPerSquare];
            int[] triangles = new int[xSize * zSize * TerrainSection.VerticesPerSquare];
            Vector2[] uvs = new Vector2[xSize * zSize * TerrainSection.VerticesPerSquare];


            Mesh mesh = new Mesh();
            GameObject overlay = new GameObject("Terrain overlay (" + sx + ", " + sz + "), " + squaresPerTexture);
            overlay.transform.parent = transform;
            overlay.AddComponent<MeshFilter>().mesh = mesh;
            overlay.AddComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];

            int i = 0;
            float y_00;
            float y_01;
            float y_10;
            float y_11;

            float worldX;
            float worldZ;

            int ux = 0;
            int uz = 0;
            float u;
            float v;

            for (int z = sz; z < sz + zSize; ++z)
            {
                ux = 0;
                for (int x = sx; x < sx + xSize; ++x)
                {
                    Vector3 currentSquare = terrainSection.GetSquareCoords(x, z);
                    Vector3 eastSquare = terrainSection.GetSquareCoords(x + 1, z);
                    Vector3 northSquare = terrainSection.GetSquareCoords(x, z + 1);
                    Vector3 northEastSquare = terrainSection.GetSquareCoords(x + 1, z + 1);
                    y_00 = currentSquare.y + 0.01f;
                    y_01 = northSquare.y + 0.01f;
                    y_10 = eastSquare.y + 0.01f;
                    y_11 = northEastSquare.y + 0.01f;


                    worldX = currentSquare.x;
                    worldZ = currentSquare.z;

                    u = ux / (float)squaresPerTexture;
                    v = uz / (float)squaresPerTexture;

                    // Bottom left triangle: (0,0) = se, (0,1) = ne, (1,0) = sw
                    uvs[i] = new Vector2(u, v);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX, y_00, worldZ);

                    uvs[i] = new Vector2(u, v + textureStep);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX, y_01, worldZ + 1);

                    uvs[i] = new Vector2(u + textureStep, v);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX + 1, y_10, worldZ);

                    // Top right triangle: (0,1) = ne, (1,1) = nw, (1,0) = se
                    uvs[i] = new Vector2(u, v + textureStep);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX, y_01, worldZ + 1);

                    uvs[i] = new Vector2(u + textureStep, v + textureStep);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX + 1, y_11, worldZ + 1);

                    uvs[i] = new Vector2(u + textureStep, v);
                    triangles[i] = i;
                    vertices[i++] = new Vector3(worldX + 1, y_10, worldZ);

                    if (++ux == squaresPerTexture)
                    {
                        ux = 0;
                    }
                }
                if (++uz == squaresPerTexture)
                {
                    uz = 0;
                }

            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
    }
}