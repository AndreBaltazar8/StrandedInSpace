using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	float positionX = 0.0f;
	float positionY = 0.0f;
	float distance = 0.5f;

	Vector3 initialPosition;

	void Start () {
		initialPosition = transform.position;
	}

	void Update () {
		GameObject rocket = GameObject.Find ("FullRocket");
		LaunchRocket rocketScript = rocket.GetComponent<LaunchRocket> ();
		if (!rocketScript.isFlying) {
			float movementX = Input.GetAxis ("Horizontal");
			float movementY = Input.GetAxis ("Vertical");
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			distance = Mathf.Clamp01 (distance - scroll * 0.3f);

			positionX = Mathf.Clamp (positionX + movementX, -30, 30);
			positionY = Mathf.Clamp (positionY + movementY, -30, 50);
			transform.position = new Vector3 (initialPosition.x + positionX, initialPosition.y, initialPosition.z + positionY) + transform.forward * (1 - distance) * 60;
		} else {
			transform.LookAt (rocket.transform);
		}
	}
}
