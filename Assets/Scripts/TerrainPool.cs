using UnityEngine;
using System.Collections.Generic;

public class TerrainPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private TerrainSegment terrainSegmentPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20;
    [SerializeField] private Transform poolContainer;
    
    private Queue<TerrainSegment> pool;
    private List<TerrainSegment> activeSegments;

    private void Awake()
    {
        if (terrainSegmentPrefab == null)
        {
            Debug.LogError("TerrainSegment prefab is not assigned!");
            enabled = false;
            return;
        }

        pool = new Queue<TerrainSegment>();
        activeSegments = new List<TerrainSegment>();

        // Create pool container if not assigned
        if (poolContainer == null)
        {
            poolContainer = new GameObject("TerrainPool").transform;
            poolContainer.SetParent(transform);
        }

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
        if (pool.Count >= maxPoolSize)
        {
            Debug.LogWarning("Pool size limit reached!");
            return;
        }

        TerrainSegment segment = Instantiate(terrainSegmentPrefab, poolContainer);
        segment.gameObject.SetActive(false);
        pool.Enqueue(segment);
    }

    public TerrainSegment GetSegment()
    {
        if (pool.Count == 0)
        {
            if (pool.Count < maxPoolSize)
            {
                CreateNewSegment();
            }
            else
            {
                Debug.LogWarning("No available segments in pool!");
                return null;
            }
        }

        TerrainSegment segment = pool.Dequeue();
        segment.gameObject.SetActive(true);
        activeSegments.Add(segment);
        return segment;
    }

    public void ReturnSegment(TerrainSegment segment)
    {
        if (segment == null) return;

        if (activeSegments.Contains(segment))
        {
            activeSegments.Remove(segment);
            segment.Reset();
            segment.gameObject.SetActive(false);
            segment.transform.SetParent(poolContainer);
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

    private void OnDestroy()
    {
        ReturnAllSegments();
    }
} 