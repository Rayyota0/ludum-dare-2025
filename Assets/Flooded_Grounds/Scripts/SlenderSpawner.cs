using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SlenderSpawner : MonoBehaviour
{
    [SerializeField] private GameObject slenderPrefab;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float scareDistance = 2f;
    [SerializeField] private float disappearAfterSeconds = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerAAAA");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger");
            SpawnSlender(other.transform);
            Destroy(gameObject); // убираем предмет
        }
    }

    private void SpawnSlender(Transform player)
    {
        // Находим случайную точку на NavMesh в радиусе
        Vector3 randomPoint = player.position + Random.insideUnitSphere * spawnRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject slenderObj = Instantiate(slenderPrefab, hit.position, Quaternion.identity);
            slenderObj.GetComponent<SlenderController>().Initialize(player);
        }
    }
}