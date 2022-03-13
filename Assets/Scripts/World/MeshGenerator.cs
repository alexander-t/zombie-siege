using UnityEngine;

namespace World
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class MeshGenerator : MonoBehaviour
    {
        public int xSize = 200;
        public int zSize = 200;

        // Indices(offsets of a square's two triangles' vertices in a one dimensional array
        const int T1_SW = 0; // i.e., the first triangle stores its (0,0) at offset 0
        const int T1_NW = 1;
        const int T1_SE = 2;
        const int T2_NW = 3;
        const int T2_NE = 4;
        const int T2_SE = 5;

        private Mesh mesh;
        private MeshCollider meshCollider;

        private TerrainSection terrainSection;
    
        void Awake()
        {
            terrainSection = new TerrainSection(xSize, zSize, transform.position);
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            GetComponent<MeshFilter>().mesh = mesh;
            meshCollider = GetComponent<MeshCollider>();
            CreateTerrainSquare();
            CreateTerrainFetures();
            UpdateMesh();
        }

        void CreateTerrainSquare()
        {
            const float PerlinDensity = 0.05f;
            const float PerlinHeight = 4;

            Vector3[] vertices = new Vector3[xSize * zSize * TerrainSection.VerticesPerSquare];
            int[] triangles = new int[xSize * zSize * TerrainSection.VerticesPerSquare];
            Vector2[] uvs = new Vector2[xSize * zSize * TerrainSection.VerticesPerSquare];

            int squaresPerTexture = 10;
            float textureStep = 1.0f / squaresPerTexture;

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

            for (int z = 0; z < zSize; ++z)
            {
                ux = 0;
                for (int x = 0; x < xSize; ++x)
                {
                    y_00 = Mathf.PerlinNoise(x * PerlinDensity, z * PerlinDensity) * PerlinHeight;
                    y_01 = Mathf.PerlinNoise(x * PerlinDensity, (z + 1) * PerlinDensity) * PerlinHeight;
                    y_10 = Mathf.PerlinNoise((x + 1) * PerlinDensity, z * PerlinDensity) * PerlinHeight;
                    y_11 = Mathf.PerlinNoise((x + 1) * PerlinDensity, (z + 1) * PerlinDensity) * PerlinHeight;

                    worldX = transform.position.x - (xSize / 2f) + x;
                    worldZ = transform.position.z - (zSize / 2f) + z;

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

            terrainSection.vertices = vertices;
            terrainSection.triangles = triangles;
            terrainSection.uvs = uvs;
            World.Instance.AddTerrainSection(0, 0, terrainSection);
        }





        void CreateTerrainFetures()
        {
            const float BaseGroundHeight = 1.5f;
            for (int z = zSize / 2 - 10; z <= zSize / 2 + 10; ++z)
            {
                for (int x = xSize / 2 - 10; x <= xSize / 2 + 10; ++x)
                {
                    SetVertexY(x, z, BaseGroundHeight);
                }
            }

            SetVertexY(1, 1, 4.0f);
            SetVertexY(1, 2, 4.0f);
            SetVertexY(2, 1, 4.0f);
            SetVertexY(2, 2, 3.0f);
            SetVertexY(5, 5, 2.0f);
        }

        public void SetSquareY(int x, int z, float y)
        {
            if (x >= xSize || z >= zSize)
            {
                throw new System.ArgumentOutOfRangeException("x or z exceeds number of squares");
            }

            for (int i = 0; i < TerrainSection.VerticesPerSquare; i++)
            {
                terrainSection.vertices[Square2VertexOffset(x, z) + i].y = y;
            }
        }



        public void SetVertexY(int x, int z, float y)
        {
            int q00 = Square2VertexOffset(x - 1, z - 1); // South west quad
            int q01 = Square2VertexOffset(x - 1, z); // North west quad
            int q10 = Square2VertexOffset(x, z - 1);
            int q11 = Square2VertexOffset(x, z);
            terrainSection.vertices[q00 + T2_NE].y = y;
            terrainSection.vertices[q01 + T1_SE].y = y;
            terrainSection.vertices[q01 + T2_SE].y = y;
            terrainSection.vertices[q11 + T1_SW].y = y;
            terrainSection.vertices[q10 + T2_NW].y = y;
            terrainSection.vertices[q10 + T1_NW].y = y;
        }

        private int Square2VertexOffset(int x, int z)
        {
            return z * xSize * TerrainSection.VerticesPerSquare + x * TerrainSection.VerticesPerSquare;
        }


        void UpdateMesh()
        {

            mesh.Clear();
            mesh.vertices = terrainSection.vertices;
            mesh.triangles = terrainSection.triangles;
            mesh.uv = terrainSection.uvs;
            mesh.Optimize();
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = mesh;
        }
    }
}