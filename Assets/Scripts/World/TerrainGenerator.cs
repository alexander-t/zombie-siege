using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace World
{

    [CustomEditor(typeof(TerrainGenerator))]
    public class TextureEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TerrainGenerator meshGenerator = (TerrainGenerator)target;
            if (GUILayout.Button("Generate"))
            {
                meshGenerator.GenerateMesh();
            }
        }
    }

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Size parameters")]
        [Tooltip("Width in squares")]
        [SerializeField] private int xSize = 200;
        [Tooltip("Depth in squares")]
        [SerializeField] private int zSize = 200;
        [SerializeField] private int squaresPerTexture = 10;
        [Header("Terrain parameters")]
        [SerializeField] private float perlinHeight = 4;
        [SerializeField] private float perlinDensity = 0.05f;
        [Header("Submesh partitioning")]
        [SerializeField] private bool partitionIntoSubMeshes = true;
        [Range(2, 10)]
        [SerializeField] private int numberOfSubMeshes = 4;
        [Header("Base parameters")]
        [SerializeField] private bool includeBase = true;
        [Tooltip("Base size in squares")]
        [SerializeField] private int baseSize = 20;
        [SerializeField] private float baseGroundHeight = 1.5f;

        // Indices(offsets) of a square's two triangles' vertices in a one dimensional array
        const int T1_SW = 0; // i.e., the first triangle stores its (0,0) at offset 0
        const int T1_NW = 1;
        const int T1_SE = 2;
        const int T2_NW = 3;
        const int T2_NE = 4;
        const int T2_SE = 5;

        private TerrainSection terrainSection;

        void Awake()
        {
            GenerateMesh();
        }
        public void GenerateMesh()
        {
            terrainSection = new TerrainSection(xSize, zSize, transform.position);

            CreateTerrainSquare();
            if (includeBase)
            {
                CreateBaseTerrain();
            }
            
            terrainSection.UpdateMesh();
            
            if (partitionIntoSubMeshes) 
            {
                PartitionIntoSubMeshes(numberOfSubMeshes);
            }
          
            terrainSection.mesh.Optimize();
            terrainSection.mesh.RecalculateNormals();
            GetComponent<MeshFilter>().sharedMesh = terrainSection.mesh;
            GetComponent<MeshCollider>().sharedMesh = terrainSection.mesh;
        }

        void CreateTerrainSquare()
        {
            Vector3[] vertices = new Vector3[xSize * zSize * TerrainSection.VerticesPerSquare];
            int[] triangles = new int[xSize * zSize * TerrainSection.VerticesPerSquare];
            Vector2[] uvs = new Vector2[xSize * zSize * TerrainSection.VerticesPerSquare];

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
                    y_00 = Mathf.PerlinNoise(x * perlinDensity, z * perlinDensity) * perlinHeight;
                    y_01 = Mathf.PerlinNoise(x * perlinDensity, (z + 1) * perlinDensity) * perlinHeight;
                    y_10 = Mathf.PerlinNoise((x + 1) * perlinDensity, z * perlinDensity) * perlinHeight;
                    y_11 = Mathf.PerlinNoise((x + 1) * perlinDensity, (z + 1) * perlinDensity) * perlinHeight;

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
            terrainSection.uvs = uvs;
            terrainSection.triangles = triangles;
            World.Instance.AddTerrainSection(0, 0, terrainSection);
        }


        private void CreateBaseTerrain()
        {
            int halfBaseSize = baseSize / 2;
            for (int z = zSize / 2 - halfBaseSize; z <= zSize / 2 + halfBaseSize; ++z)
            {
                for (int x = xSize / 2 - halfBaseSize; x <= xSize / 2 + halfBaseSize; ++x)
                {
                    SetVertexY(x, z, baseGroundHeight);
                }
            }
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


        /// <summary>
        /// Sets a vertex's Y coordinate. The challenge is that a vertex at the center of  
        /// four quads is part of six triangles.
        /// </summary>
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

        /// <summary>
        /// Partitions the terrain section's mesh into the specifified number of submeshes along the texture boundaries.
        /// Note that the <c>squaresPerTexture</c> variables specifies how many squares a single texture will span.
        /// The value of this variable is thus quite critical to the partitioning.
        /// </summary>
        private void PartitionIntoSubMeshes(int subMeshCount) 
        {
            List<int>[] destList = new List<int>[subMeshCount];
            for (int i = 0; i < subMeshCount; ++i)
            {
                destList[i] = new List<int>();
            }

            // Used for "calculating back" x and z squares from a 1-dimensional array of triangles
            int z = 0;
            int x = 0;

            // Partitioning is made along texture boundaries. These hold the square number as per texture size.
            int xSquare;
            int zSquare;
            for (int i = 0; i < terrainSection.triangles.Length; i += TerrainSection.VerticesPerSquare)
            {
                if (x >= xSize)
                {
                    x = 0;
                    ++z;
                }

                xSquare = x / squaresPerTexture;
                zSquare = z / squaresPerTexture;

                for (int j = i; j < i + TerrainSection.VerticesPerSquare; j++)
                {
                    destList[(xSquare + zSquare) % subMeshCount].Add(terrainSection.triangles[j]);
                }

                x++;
            }
            terrainSection.mesh.subMeshCount = subMeshCount;
            for (int i = 0; i < subMeshCount; ++i)
            {
                terrainSection.mesh.SetTriangles(destList[i].ToArray(), i);
            }
        }
    }
}