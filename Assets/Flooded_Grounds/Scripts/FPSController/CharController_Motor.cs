using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


	void Start(){
		//LockCursor ();
		character = GetComponent<CharacterController> ();
		if (Application.isEditor) {
			webGLRightClickRotation = false;
			sensitivity = sensitivity * 1.5f;
		}
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

	void Die() {
		isDead = true;
		Debug.Log("Игрок умер от ловушки!");
		// Здесь можно добавить эффекты смерти, звуки, анимации и т.д.
	}



	void Update(){
		// Если игрок мертв, блокируем все движения
		if (isDead) {
			return;
		}

		moveFB = Input.GetAxis ("Horizontal") * speed;
		moveLR = Input.GetAxis ("Vertical") * speed;

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		//rotX = Input.GetKey (KeyCode.Joystick1Button4);
		//rotY = Input.GetKey (KeyCode.Joystick1Button5);

		CheckForWaterHeight ();

		if (character.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
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
