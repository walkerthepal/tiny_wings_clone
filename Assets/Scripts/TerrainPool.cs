using UnityEngine;
using System.Collections.Generic;

public class TerrainPool : MonoBehaviour
{
    [SerializeField] private TerrainSegment terrainSegmentPrefab;
    [SerializeField] private int initialPoolSize = 10;
    
    private Queue<TerrainSegment> pool;
    private List<TerrainSegment> activeSegments;

    private void Awake()
    {
        pool = new Queue<TerrainSegment>();
        activeSegments = new List<TerrainSegment>();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewSegment();
        }
    }

    private void CreateNewSegment()
    {
        TerrainSegment segment = Instantiate(terrainSegmentPrefab, transform);
        segment.gameObject.SetActive(false);
        pool.Enqueue(segment);
    }

    public TerrainSegment GetSegment()
    {
        if (pool.Count == 0)
        {
            CreateNewSegment();
        }

        TerrainSegment segment = pool.Dequeue();
        segment.gameObject.SetActive(true);
        activeSegments.Add(segment);
        return segment;
    }

    public void ReturnSegment(TerrainSegment segment)
    {
        if (activeSegments.Contains(segment))
        {
            activeSegments.Remove(segment);
            segment.Reset();
            segment.gameObject.SetActive(false);
            pool.Enqueue(segment);
        }
    }

    public void ReturnAllSegments()
    {
        foreach (TerrainSegment segment in activeSegments.ToArray())
        {
            ReturnSegment(segment);
        }
    }
} 