using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGroup : MonoBehaviour {

	public GameObject asteroidObject;
	private int targetX;
	private int targetY;
	private Building targetBuilding;
	private int numAsteroids;

	void Start () {
		this.numAsteroids = Random.Range(4, 12);
		for (var i = 0; i < numAsteroids; ++i) {
			GameObject asteroid = Instantiate (asteroidObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			asteroid.transform.localScale = new Vector3 (Random.value * 0.6f + 0.7f, Random.value * 0.6f + 0.7f, Random.value * 0.6f + 0.7f);
			asteroid.transform.SetParent (transform, false);
			asteroid.transform.localPosition = new Vector3 (Random.value * 10, Random.value * 10, Random.value * 10);

			Rigidbody asteroidBody = asteroid.GetComponent<Rigidbody> ();

			float T = Mathf.Sqrt (asteroid.transform.position.y * 2 / -Physics.gravity.y);

			float ax = targetX + Random.value * 10 - 5;
			float ay = targetY + Random.value * 10 - 5;

			asteroidBody.velocity = new Vector3 ((ax - asteroid.transform.position.x) / T, 0, (ay - asteroid.transform.position.z) / T);
			asteroid.GetComponent<Asteroid> ().HitEvent += new Asteroid.AsteroidHitEventHandler (AsteroidHit);
		}
	}

	public void AsteroidHit(bool damage) {
		numAsteroids--;
		if (numAsteroids == 0)
			Destroy (gameObject);
		if (damage)
			targetBuilding.Hit ();
	}

	public void SetTarget(int x, int y, Building targetBuilding) {
		targetX = x;
		targetY = y;
		this.targetBuilding = targetBuilding;
	}
}
