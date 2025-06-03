using UnityEngine;

public class TerrainSegment : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private EdgeCollider2D edgeCollider;
    private Vector2[] points;
    private float width;
    private float height;

    private void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    }

    public void Initialize(float width, float height, Vector2[] points)
    {
        this.width = width;
        this.height = height;
        this.points = points;
        GenerateMesh();
        UpdateCollider();
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        
        // Create vertices
        Vector3[] vertices = new Vector3[points.Length * 2];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);
            vertices[i + points.Length] = new Vector3(points[i].x, points[i].y - height, 0);
        }

        // Create triangles
        int[] triangles = new int[(points.Length - 1) * 6];
        for (int i = 0; i < points.Length - 1; i++)
        {
            int baseIndex = i * 6;
            triangles[baseIndex] = i;
            triangles[baseIndex + 1] = i + 1;
            triangles[baseIndex + 2] = i + points.Length;
            triangles[baseIndex + 3] = i + 1;
            triangles[baseIndex + 4] = i + points.Length + 1;
            triangles[baseIndex + 5] = i + points.Length;
        }

        // Create UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < points.Length; i++)
        {
            uvs[i] = new Vector2((float)i / (points.Length - 1), 1);
            uvs[i + points.Length] = new Vector2((float)i / (points.Length - 1), 0);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void UpdateCollider()
    {
        edgeCollider.points = points;
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
} 