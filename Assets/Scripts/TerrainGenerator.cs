using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] private float segmentWidth = 20f;
    [SerializeField] private float segmentHeight = 10f;
    [SerializeField] private int pointsPerSegment = 20;
    [SerializeField] private float heightScale = 5f;
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float smoothness = 0.5f;
    
    [Header("References")]
    [SerializeField] private TerrainPool terrainPool;
    [SerializeField] private Material terrainMaterial;
    
    private float currentX = 0f;
    private float noiseOffset = 0f;
    private List<TerrainSegment> activeSegments = new List<TerrainSegment>();
    private const int MAX_SEGMENTS = 5;

    private void Start()
    {
        GenerateInitialTerrain();
    }

    private void GenerateInitialTerrain()
    {
        for (int i = 0; i < MAX_SEGMENTS; i++)
        {
            GenerateNextSegment();
        }
    }

    public void GenerateNextSegment()
    {
        // Get a segment from the pool
        TerrainSegment segment = terrainPool.GetSegment();
        segment.transform.position = new Vector3(currentX, 0, 0);
        
        // Generate points for the segment
        Vector2[] points = GenerateSegmentPoints();
        segment.Initialize(segmentWidth, segmentHeight, points);
        segment.SetMaterial(terrainMaterial);
        
        activeSegments.Add(segment);
        currentX += segmentWidth;
        noiseOffset += noiseScale;
    }

    private Vector2[] GenerateSegmentPoints()
    {
        Vector2[] points = new Vector2[pointsPerSegment];
        float step = segmentWidth / (pointsPerSegment - 1);

        for (int i = 0; i < pointsPerSegment; i++)
        {
            float x = i * step;
            float noise = Mathf.PerlinNoise(x * noiseScale + noiseOffset, 0);
            float height = noise * heightScale;
            
            // Apply smoothing
            if (i > 0 && i < pointsPerSegment - 1)
            {
                float prevHeight = points[i - 1].y;
                height = Mathf.Lerp(height, prevHeight, smoothness);
            }
            
            points[i] = new Vector2(x, height);
        }

        return points;
    }

    private void Update()
    {
        // Check if we need to generate more terrain
        if (activeSegments.Count < MAX_SEGMENTS)
        {
            GenerateNextSegment();
        }

        // Check if we need to remove old segments
        if (activeSegments.Count > 0)
        {
            TerrainSegment firstSegment = activeSegments[0];
            if (firstSegment.transform.position.x < -segmentWidth)
            {
                terrainPool.ReturnSegment(firstSegment);
                activeSegments.RemoveAt(0);
            }
        }
    }

    public void Reset()
    {
        terrainPool.ReturnAllSegments();
        activeSegments.Clear();
        currentX = 0f;
        noiseOffset = 0f;
        GenerateInitialTerrain();
    }
} 