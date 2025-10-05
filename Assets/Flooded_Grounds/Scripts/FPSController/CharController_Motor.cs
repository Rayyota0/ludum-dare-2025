using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharController_Motor : MonoBehaviour {

	public float speed = 5.0f;
	public float sensitivity = 30.0f;
	public float WaterHeight = 15.5f;
	public float jumpForce = 4.0f;
	CharacterController character;
	public GameObject cam;
	float moveFB, moveLR;
	float rotX, rotY;
	public bool webGLRightClickRotation = true;
	float gravity = -9.8f;
	float verticalVelocity = 0f;
	bool isDead = false;
	
	[Header("Death Effects")]
	public Image fadeImage;
	public float fadeDuration = 0.5f;

	private bool isFrozenBySlender = false;
	private Quaternion targetCamRotation;
	private Quaternion targetPlayerRotation;

	// Вызывается из SlenderController
	public void FreezeAndLookAt(Vector3 slenderPosition)
	{
		isFrozenBySlender = true;

		// 1. Поворот игрока по горизонтали к Слендеру
		Vector3 playerToTarget = slenderPosition - transform.position;
		targetPlayerRotation = Quaternion.LookRotation(playerToTarget);

		// Для камеры — смотрим точно на точку (включая высоту)
		Vector3 camToTarget = slenderPosition - cam.transform.position;
		targetCamRotation = Quaternion.LookRotation(camToTarget);

		// 2. Поворот камеры (вверх/вниз) к Слендеру
		Vector3 camToSlender = slenderPosition - cam.transform.position;
		targetCamRotation = Quaternion.LookRotation(camToSlender);
	}

	void Start(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		character = GetComponent<CharacterController> ();
		if (Application.isEditor) {
			webGLRightClickRotation = false;
			sensitivity = sensitivity * 1.5f;
		}
		
		// Создаем fadeImage если не назначен
		if (fadeImage == null) {
			CreateFadeImage();
		}
	}
	
	void CreateFadeImage() {
		// Находим Canvas
		Canvas canvas = FindObjectOfType<Canvas>();
		if (canvas == null) {
			GameObject canvasGO = new GameObject("Canvas");
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
		}
		
		// Создаем Image
		GameObject fadeGO = new GameObject("FadeImage");
		fadeGO.transform.SetParent(canvas.transform, false);
		
		fadeImage = fadeGO.AddComponent<Image>();
		fadeImage.color = new Color(0, 0, 0, 0); // Прозрачный черный
		
		// Настраиваем RectTransform для полноэкранного размера
		RectTransform rect = fadeImage.GetComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		
		Debug.Log("FadeImage создан автоматически");
	}


	void CheckForWaterHeight(){
		if (transform.position.y < WaterHeight) {
			gravity = 0f;
			verticalVelocity = 0f;
		} else {
			gravity = -9.8f;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.CompareTag("trap") && !isDead) {
			Die();
		}
	}

	public void Die() {
		isDead = true;
		Debug.Log("Игрок умер от ловушки!");
		
		StartCoroutine(DeathSequence());
		// Здесь можно добавить эффекты смерти, звуки, анимации и т.д.
	}
	
	IEnumerator DeathSequence() {
		Debug.Log("Начинаем последовательность смерти");
		
		// Начинаем потемнение
		if (fadeImage != null) {
			Debug.Log("Запускаем потемнение");
			yield return StartCoroutine(FadeToBlack());
			Debug.Log("Потемнение завершено");
		} else {
			Debug.LogWarning("FadeImage не назначен! Пропускаем потемнение");
		}
		
		// Небольшая задержка перед переходом
		yield return new WaitForSeconds(0.5f);
		
		Debug.Log("Переходим на сцену смерти");
		// Переходим на сцену смерти асинхронно
		yield return SceneManager.LoadSceneAsync("DieScene");
	}
	
	IEnumerator FadeToBlack() {
		Debug.Log("Начинаем FadeToBlack");
		Color fadeColor = fadeImage.color;
		float elapsedTime = 0f;
		
		while (elapsedTime < fadeDuration) {
			elapsedTime += Time.deltaTime;
			float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
			fadeColor.a = alpha;
			fadeImage.color = fadeColor;
			Debug.Log($"Fade progress: {alpha:F2}");
			yield return null;
		}
		
		// Убеждаемся, что экран полностью черный
		fadeColor.a = 1f;
		fadeImage.color = fadeColor;
		Debug.Log("FadeToBlack завершен");
	}



	void Update(){
		// Если игрок мертв, блокируем все движения
		if (isDead || isFrozenBySlender)
    {
        if (isFrozenBySlender)
        {
            // Плавно поворачиваем игрока и камеру к Слендеру
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPlayerRotation, Time.deltaTime * 5f);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetCamRotation, Time.deltaTime * 5f);
        }
        return;
    }

		moveFB = Input.GetAxis ("Horizontal") * speed;
		moveLR = Input.GetAxis ("Vertical") * speed;

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		//rotX = Input.GetKey (KeyCode.Joystick1Button4);
		//rotY = Input.GetKey (KeyCode.Joystick1Button5);

		CheckForWaterHeight ();

		if (Input.GetKeyDown(KeyCode.Space)) {
			verticalVelocity = jumpForce;
		}

		verticalVelocity += gravity * Time.deltaTime;

		Vector3 movement = new Vector3 (moveFB, verticalVelocity, moveLR);

		if (webGLRightClickRotation) {
			if (Input.GetKey (KeyCode.Mouse0)) {
				CameraRotation (cam, rotX, rotY);
			}
		} else if (!webGLRightClickRotation) {
			CameraRotation (cam, rotX, rotY);
		}

		movement = transform.rotation * movement;
		character.Move (movement * Time.deltaTime);
	}


	void CameraRotation(GameObject cam, float rotX, float rotY){		
		transform.Rotate (0, rotX * Time.deltaTime, 0);
		cam.transform.Rotate (-rotY * Time.deltaTime, 0, 0);
	}




}
