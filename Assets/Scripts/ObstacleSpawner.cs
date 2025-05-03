using UnityEngine;
using Controller; 
public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // 돌, 가짜마늘
    public float spawnRateMin = 0.5f; // 최소 생성 주기
    public float spawnRateMax = 3f; // 최대 생성 주기
    
    private Transform target;
    private float spawnRate; // 생성 주기
    private float timeAfterSpawn; // 최근 생성 시점에서 지난 시간
   
    void Start()
    {
        // 최근 생성 이후의 누적 시간을 0으로 초기화
        timeAfterSpawn = 0f;
        // 총알 생성 간격을 spawnRateMin과 spawnRateMax 사이에서 랜덤 지정 
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        // PlayerController 컴포넌트를 가진 게임 오브젝트를 찾아 조준 대상으로 설정
        target = FindObjectOfType<PlayerController>().transform;
    }
    void Update()
    {
         // timeAfterSpawn을 갱신
        timeAfterSpawn += Time.deltaTime;
        // 최근 생성 시점에서부터 누적된 시간이, 생성 주기보다 크거나 같다면
        if (timeAfterSpawn >= spawnRate)
        {   // 누적된 시간을 리셋
            timeAfterSpawn = 0f;
            float shootY = 1f; // Y축으로 살짝 위 

           // 랜덤 프리팹 선택 (돌 or 가짜 마늘)
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
             // 생성
            GameObject obj = Instantiate(prefab, 
                transform.position + Vector3.up * shootY, transform.rotation);
            
            obj.transform.LookAt(target);

            // 다음 주기 설정
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}
