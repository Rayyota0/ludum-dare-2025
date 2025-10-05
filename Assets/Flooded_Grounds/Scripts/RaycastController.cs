using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RaycastController : MonoBehaviour
{
    [Header("Collection Features")]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI errorText;
    public Camera mainCamera;
    public int maxItems = 4;

    [Header("Raycast Features")]
    [SerializeField] private float rayLength = 5;

    private NoteController _noteController;
    private Gravestone _gravestone;

    [Header("Crosshair")]
    [SerializeField] private Image crosshair;
    [SerializeField] private KeyCode interactKey;

    [SerializeField] private GameObject slenderPrefab;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float scareDistance = 2f;
    [SerializeField] private float disappearAfterSeconds = 10f;

    void Start()
    {
    }

    void Update()
    {
        // Collection system (right mouse button)
        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("ЛКМ нажата");
            if (mainCamera == null) return;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit collectionHit;
            if (Physics.Raycast(ray, out collectionHit, 5f))
            {
                Debug.Log("Hit объект: " + collectionHit.transform.name);
                if (collectionHit.transform.CompareTag("PickUp"))
                {
                    Destroy(collectionHit.transform.gameObject);
                    score++;
                    SpawnSlender(this.transform);
                    Debug.Log("Подобран предмет. Счет: " + score);
                    if (scoreText != null)
                        scoreText.text = "Looted: " + score.ToString() + " / 4";
                }
            }
        }

        // Interaction system
        if (Physics.Raycast(mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayLength))
        {
            var readebleItem = hit.collider.GetComponent<NoteController>();
            var gravestoneItem = hit.collider.GetComponent<Gravestone>();
            
            if (readebleItem != null)
            {
                _noteController = readebleItem;
                _gravestone = null;
                HighlightCrosshair(true);
            }
            else if (gravestoneItem != null || hit.transform.CompareTag("Gravestone"))
            {
                _gravestone = gravestoneItem != null ? gravestoneItem : hit.transform.GetComponent<Gravestone>();
                _noteController = null;
                HighlightCrosshair(true);
            }
            else
            {
                ClearInteractions();
            }
        }
        else
        {
            ClearInteractions();
        }

        if (_noteController != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                _noteController.ShowNote();
            }
        }
        else if (_gravestone != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                
                if (score >= maxItems)
                {
                    // Load victory scene
                    SceneManager.LoadScene("StartMenu");
                }
                else
                {
                    // Show progress message
                    Debug.Log($"Собрано {score} из {maxItems} предметов");
                    if (errorText != null)
                        errorText.text = "Иди собирай";
                }
            }
        }
    }

    void ClearInteractions()
    {
        if (_noteController != null || _gravestone != null)
        {
            HighlightCrosshair(false);
            _noteController = null;
            _gravestone = null;
        }
    }

    void HighlightCrosshair(bool on)
    {
        if (on)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

    private void SpawnSlender(Transform player)
    {
        // Находим случайную точку на NavMesh в радиусе
        Vector3 randomPoint = player.position + Random.insideUnitSphere * spawnRadius;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, UnityEngine.AI.NavMesh.AllAreas))
        {
            GameObject slenderObj = Instantiate(slenderPrefab, hit.position, Quaternion.identity);
            slenderObj.GetComponent<SlenderController>().Initialize(player);
        }
    }
}
