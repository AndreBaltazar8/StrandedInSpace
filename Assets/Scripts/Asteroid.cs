using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
	public delegate void AsteroidHitEventHandler (bool damage);
	public AsteroidHitEventHandler HitEvent;

	public GameObject missileIncoming;

	public bool IsBeingTargeted() {
		return missileIncoming != null;
	}

	public void SetMissileIncoming(GameObject missile) {
		missileIncoming = missile;
	}

	void OnCollisionEnter(Collision col) {
		if (HitEvent != null)
			HitEvent (col.gameObject.tag != "Missile"); // damage if not missile
		Destroy (gameObject);
	}

	void Update() {
	}
}
