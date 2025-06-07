using UnityEngine;

public class TerrainSegment : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private EdgeCollider2D edgeCollider;

    [Header("Settings")]
    [SerializeField] private Material defaultMaterial;
    
    private Vector2[] points;
    private float width;
    private float height;

    private void Awake()
    {
        // Only add components if they don't exist
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (edgeCollider == null) edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        
        // Set default material if none is assigned
        if (meshRenderer.material == null && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }
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
        if (material != null)
        {
            meshRenderer.material = material;
        }
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        if (meshFilter.mesh != null)
        {
            meshFilter.mesh.Clear();
        }
        points = null;
    }
} 