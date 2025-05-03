using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller; 
public class GarlicSpawner : MonoBehaviour {
    public GameObject garlicPrefab; // 생성할 총알의 원본 프리팹
    public float spawnRateMin = 0.5f; // 최소 생성 주기
    public float spawnRateMax = 3f; // 최대 생성 주기
    public float throwForce = 5f; // 발사 속도

    private Transform target; // 발사할 대상
    private float spawnRate; // 생성 주기
    private float timeAfterSpawn; // 최근 생성 시점에서 지난 시간

    void Start() {
        // 최근 생성 이후의 누적 시간을 0으로 초기화
        timeAfterSpawn = 0f;
        // 총알 생성 간격을 spawnRateMin과 spawnRateMax 사이에서 랜덤 지정 
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        // PlayerController 컴포넌트를 가진 게임 오브젝트를 찾아 조준 대상으로 설정
        target = FindObjectOfType<PlayerController>().transform;
    }

    void Update() {
        // timeAfterSpawn을 갱신
        timeAfterSpawn += Time.deltaTime;
        float shootY = 1f; // Y축으로 살짝 위 
        // 최근 생성 시점에서부터 누적된 시간이, 생성 주기보다 크거나 같다면
        if (timeAfterSpawn >= spawnRate)
        {
            // 누적된 시간을 리셋
            timeAfterSpawn = 0f;
            // 마늘 생성
            GameObject garlic = Instantiate(garlicPrefab,
                transform.position + Vector3.up * shootY, Quaternion.identity);
            
            // 360도 랜덤 방향 설정
            float angle = Random.Range(0f, 360f);   
            float rad = angle * Mathf.Deg2Rad;
            Vector3 randomDir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)).normalized;

            // 마늘 회전 방향을 해당 방향으로 설정
            garlic.transform.rotation = Quaternion.LookRotation(randomDir);


            // 힘 적용
            Rigidbody rb = garlic.GetComponent<Rigidbody>();
            if (rb != null)
            {
                
                rb.AddForce(randomDir * throwForce, ForceMode.Impulse);
            }

            // 다음번 생성 간격을 spawnRateMin, spawnRateMax 사이에서 랜덤 지정
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}