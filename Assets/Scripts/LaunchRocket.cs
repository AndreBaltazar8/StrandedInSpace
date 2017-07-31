using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchRocket : MonoBehaviour {

	public bool isFlying = false;

	public void MakeRocketFly() {
		isFlying = true;
		GameObject.Find ("GameController").GetComponent<GameController>().EndGame ();
	}

	void FixedUpdate() {
		if (isFlying) {
			GetComponent<Rigidbody> ().AddForce (Vector3.up, ForceMode.VelocityChange);
		}
	}

}
