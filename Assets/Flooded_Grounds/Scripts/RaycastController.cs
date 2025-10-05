using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaycastCollector : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("ЛКМ нажата");
            if (mainCamera == null) return;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5f))
            {
                Debug.Log("Hit объект: " + hit.transform.name);
                if (hit.transform.CompareTag("PickUp"))
                {
                    Destroy(hit.transform.gameObject);
                    score++;
                    Debug.Log("Подобран предмет. Счет: " + score);
                    if (scoreText != null)
                        scoreText.text = "Looted: " + score.ToString() + " / 4";
                }
            }
        }
    }
}
