using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	public GameObject asteroidTarget;
	private Rigidbody rigidBody;

	private float initialLaunch = 0.2f;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		if (initialLaunch > 0.0f) {
			rigidBody.AddForce (transform.forward * 200);
			initialLaunch -= Time.fixedDeltaTime;
		} else {
			if (asteroidTarget != null) {
				transform.LookAt (asteroidTarget.transform);
				rigidBody.velocity = (asteroidTarget.transform.position - transform.position).normalized * 80;
			} else
				Destroy (gameObject);
		}
	}

	public void SetTarget(GameObject asteroid) {
		asteroidTarget = asteroid;
	}

	void OnCollisionEnter() {
		Destroy (gameObject);
	}
}
