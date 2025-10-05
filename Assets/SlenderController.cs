using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SlenderController : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 4f;
    public float scareDistance = 2f;
    public float disappearAfter = 30f;

    public Transform lookTarget;
    private Transform player;
    private Animator animator;
    private AudioSource audioSource;
    private bool isScaring = false;

    public void Initialize(Transform playerTarget)
    {
        player = playerTarget;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("isRunning");
        // Запускаем таймер исчезновения
        StartCoroutine(DisappearAfterDelay());
    }

    void Update()
    {
        if (isScaring || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= scareDistance)
        {
            StartScareSequence();
        }
        else
        {
            // Двигаемся к игроку
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(player.position);
        }
    }

   void StartScareSequence()
{
    isScaring = true;

    // Блокируем игрока и заставляем смотреть на Слендера
    var playerMotor = player.GetComponent<CharController_Motor>();
    
    if (playerMotor != null)
    {
        playerMotor.FreezeAndLookAt(lookTarget.position);
    }
    
    if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    // Анимация испуга
    if (animator != null)
        animator.SetTrigger("Scare");

    // Запускаем смерть через 2.5 сек
    Invoke("KillPlayer", 2.5f);
}

void KillPlayer()
{
    var playerMotor = player.GetComponent<CharController_Motor>();
    if (playerMotor != null)
    {
            playerMotor.Die(); // используем уже существующий метод смерти!
    }
    else
    {
        SceneManager.LoadScene("DieScene");
    }
}

    IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearAfter);
        if (!isScaring)
        {
            Destroy(gameObject);
        }
    }

    void LoadGameOver()
    {
        SceneManager.LoadScene("DieScene");
    }
}