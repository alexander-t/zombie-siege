using UnityEngine;

namespace World
{

    /*
     * A terrain section is a large chunk of the world, something like 200x200 meters.
     */
    public class TerrainSection
    {
        public const int VerticesPerSquare = 6;

        // These arrays are needed because the mesh's vertices cannot be modified directly, i.e
        // you can't do terrainSection.mesh.vertices[idx].y = y
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;
        
        public Mesh mesh;

        private int xSize;
        private int zSize;
        private Vector3 worldPosition;

        public TerrainSection(int xSize, int zSize, Vector3 worldPosition)
        {
            this.xSize = xSize;
            this.zSize = zSize;
            this.worldPosition = worldPosition;

            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.Clear();
        }

        public int XSize
        {
            get { return xSize; }
        }
        public int ZSize
        {
            get { return zSize; }
        }

        /// <summary>
        /// Updates this section's mesh by setting the values from the public vertices, uvs, and triangles arrays.
        /// </summary>
        public void UpdateMesh()
        {
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }

        public Vector3 GetSquareCoords(int x, int z)
        {
            return vertices[Square2VertexOffset(x, z)];
        }


        public Vector3 GetWorldCoords(int x, int z)
        {
            return new Vector3(worldPosition.x - (xSize / 2f) + x,
                              vertices[Square2VertexOffset(x, z)].y,
                              worldPosition.z - (zSize / 2f) + z);
        }


        public bool OffLimits(int x, int z)
        {
            return (x >= xSize / 2 - 10 && x <= xSize / 2 + 10) && (z >= zSize / 2 - 10 && z <= zSize / 2 + 10);
        }


        private int Square2VertexOffset(int x, int z)
        {
            return z * xSize * VerticesPerSquare + x * VerticesPerSquare;
        }
    }

    public class World
    {
        private static World instance;
        private TerrainSection terrainSection;

        private World()
        {
        }

        public static World Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new World();
                }
                return instance;
            }
        }

        public void AddTerrainSection(int tx, int tz, TerrainSection terrainSection)
        {
            this.terrainSection = terrainSection;
        }

        public TerrainSection GetTerrainSection(int tx, int tz)
        {
            return terrainSection;
        }
    }
}
