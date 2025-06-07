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
    [SerializeField] private float minHeight = -2f;
    [SerializeField] private float maxHeight = 8f;
    
    [Header("References")]
    [SerializeField] private TerrainPool terrainPool;
    [SerializeField] private Material terrainMaterial;
    [SerializeField] private Transform playerTransform;
    
    private float currentX = 0f;
    private float noiseOffset = 0f;
    private List<TerrainSegment> activeSegments = new List<TerrainSegment>();
    private const int MAX_SEGMENTS = 5;
    private const float GENERATION_DISTANCE = 40f;

    private void Start()
    {
        if (terrainPool == null)
        {
            Debug.LogError("TerrainPool reference is missing!");
            enabled = false;
            return;
        }

        if (terrainMaterial == null)
        {
            Debug.LogWarning("Terrain material is not assigned!");
        }

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
        TerrainSegment segment = terrainPool.GetSegment();
        if (segment == null) return;

        segment.transform.position = new Vector3(currentX, 0, 0);
        
        Vector2[] points = GenerateSegmentPoints();
        segment.Initialize(segmentWidth, segmentHeight, points);
        
        if (terrainMaterial != null)
        {
            segment.SetMaterial(terrainMaterial);
        }
        
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
            float height = Mathf.Lerp(minHeight, maxHeight, noise);
            
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
        if (playerTransform == null) return;

        float playerX = playerTransform.position.x;
        
        // Generate new segments ahead of the player
        while (currentX < playerX + GENERATION_DISTANCE)
        {
            GenerateNextSegment();
        }

        // Remove segments that are too far behind
        while (activeSegments.Count > 0)
        {
            TerrainSegment firstSegment = activeSegments[0];
            if (firstSegment.transform.position.x < playerX - GENERATION_DISTANCE)
            {
                terrainPool.ReturnSegment(firstSegment);
                activeSegments.RemoveAt(0);
            }
            else
            {
                break;
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