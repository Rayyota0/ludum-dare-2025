using UnityEngine;

public class OneTimeSound : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("=== ТРИГГЕР СРАБОТАЛ ===");
        Debug.Log($"Объект вошедший в триггер: {other.gameObject.name}");
        Debug.Log($"hasPlayed = {hasPlayed}");
        
        // Ищем Camera в дочерних объектах FpsController
        Camera camera = other.GetComponentInChildren<Camera>();
        
        if (camera != null)
        {
            Debug.Log($"✅ Найдена камера: {camera.name}");
            Debug.Log($"Камера является основной: {camera.CompareTag("MainCamera")}");
            
            if (!hasPlayed)
            {
                Debug.Log("🎵 Воспроизводим звук!");
                audioSource.Play();
                hasPlayed = true;
            }
            else
            {
                Debug.Log("ℹ️ Звук уже был воспроизведен ранее");
            }
        }
        else
        {
            Debug.Log("❌ Камера не найдена в дочерних объектах");
        }
    }
}