using System.Collections;
using UnityEngine;

public class ObstaclePersonSpawner : MonoBehaviour
{
    public static ObstaclePersonSpawner Instance;
    public GameObject personObstaclePrefab;
    public Vector3[] spawnPositions; // 인스펙터에서 직접 지정하거나 코드에서 자동 지정 가능
    private Transform levelTransform;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        float pos = 20f; // 맵 바깥쪽 벽 기준 좌표

        spawnPositions = new Vector3[]
        {
            new Vector3(-pos, 0, -pos),
            new Vector3(-pos, 0,  pos),
            new Vector3( pos, 0, -pos),
            new Vector3( pos, 0,  pos),
            new Vector3(-pos, 0,   0),
            new Vector3( pos, 0,   0),
            new Vector3(  0, 0,  pos),
            new Vector3(  0, 0, -pos),
        };

        // Level 오브젝트 찾기
        GameObject levelObj = GameObject.Find("Level");
        if (levelObj != null)
        {
            levelTransform = levelObj.transform;
        }
        else
        {
            Debug.LogWarning("Level 오브젝트를 찾을 수 없습니다!");
        }
    }
     public void SpawnObstaclePersons(int stage)
    {
        if (levelTransform == null) return;
        int spawnCount = Mathf.Min((int)Mathf.Pow(2, stage - 1), spawnPositions.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            // Level 기준으로 회전된 위치 계산
            Vector3 worldPos = levelTransform.TransformPoint(spawnPositions[i]);
             // 프리팹 생성 + 부모를 Level로 지정
            GameObject obj = Instantiate(personObstaclePrefab, worldPos, Quaternion.identity, levelTransform);
        }
    }
}