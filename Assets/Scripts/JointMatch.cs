using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointMatch : MonoBehaviour {

	public Transform from;
	Quaternion rotation;

	void Start () {
		rotation = transform.rotation;
	}

	void Update () {
		transform.SetPositionAndRotation (from.position, from.rotation);
		transform.Rotate (rotation.eulerAngles);
		Debug.DrawLine (transform.position, transform.position + transform.up);
	}
}
