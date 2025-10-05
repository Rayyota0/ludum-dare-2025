using UnityEngine;

public class TestItemScript : MonoBehaviour
{
    [SerializeField] public GameObject slenderPrefab;
    [SerializeField] public float spawnRadius = 50f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnSlender(other.transform);
            Destroy(gameObject);
        }
    }

    void SpawnSlender(Transform player)
    {
        // Генерируем случайную точку в радиусе
        Vector3 randomDir = Random.insideUnitSphere * spawnRadius;
        randomDir.y = 0; // держим на земле (если уровень плоский)
        Vector3 spawnPos = player.position + randomDir;

        // Создаём Слендера
        GameObject slender = Instantiate(slenderPrefab, spawnPos, Quaternion.identity);
        slender.GetComponent<SlenderController>().Initialize(player);
    }
}