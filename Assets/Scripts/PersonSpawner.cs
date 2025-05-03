using System.Collections;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    public static PersonSpawner Instance;
    public GameObject personPrefab;
    public Vector3[] spawnPositions;
    
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        float offset = 10f;

        spawnPositions = new Vector3[]
        {
            new Vector3(-offset, 0, -offset),
            new Vector3(-offset, 0,  offset),
            new Vector3( offset, 0, -offset),
            new Vector3( offset, 0,  offset),
        };
    }
     public void SpawnGarlicPersons(int stage)
    {
        // 최대 4개까지만 생성
        int spawnCount = Mathf.Min(stage, spawnPositions.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            // Level 의 회전을 위해 위치 재계산 
            Vector3 rotatedPosi = transform.TransformPoint(spawnPositions[i]);
            Instantiate(personPrefab, rotatedPosi, Quaternion.identity);
        }
    }
}
