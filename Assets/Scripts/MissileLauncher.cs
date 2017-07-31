using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour {

	public GameObject rocketObject;
	public GameObject missileSlot1;
	public GameObject missileSlot2;

	private float lastLaunchOne = 0;
	private float lastLaunchTwo = 0;

	void Start () {
	}

	void Update () {
		if (GetComponentInParent<BuildingObject> ().GetBuilding ().IsInConstruction ())
			return;

		GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
		foreach (GameObject asteroid in asteroids) {
			Asteroid asteroidScript = asteroid.GetComponent<Asteroid> ();
			if (!asteroidScript.IsBeingTargeted ()) {
				if (lastLaunchOne + 4 < Time.time) {
					if (GameObject.Find ("GameController").GetComponent<GameController>().PurchaseMissile ()) {
						GameObject missileOne = Instantiate (rocketObject, missileSlot1.transform);
						missileOne.transform.SetParent (null);
						asteroidScript.SetMissileIncoming (missileOne);
						missileOne.GetComponent<Missile> ().SetTarget (asteroid);
						lastLaunchOne = Time.time;
					}
				} else if (lastLaunchTwo + 4 < Time.time) {
					if (GameObject.Find ("GameController").GetComponent<GameController>().PurchaseMissile ()) {
						GameObject missileTwo = Instantiate (rocketObject, missileSlot2.transform);
						missileTwo.transform.SetParent (null);
						asteroidScript.SetMissileIncoming (missileTwo);
						missileTwo.GetComponent<Missile> ().SetTarget (asteroid);
						lastLaunchTwo = Time.time;
					}
				}
			}
		}
	}
}
