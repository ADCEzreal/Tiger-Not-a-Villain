using UnityEngine;

public class itemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public float spawnInterval = 10f;
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            Vector3 pos = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                1f,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Instantiate(itemPrefab, pos, Quaternion.identity);
        }
    }
}
