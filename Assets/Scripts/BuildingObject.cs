using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingObject : MonoBehaviour {

	private Building building;

	public Building GetBuilding() {
		return building;
	}

	public void Init(Building building) {
		this.building = building;
	}

	public void InitConstruction(int constructionTime) {
		StartCoroutine (ConstructionRoutine(constructionTime));
	}

	IEnumerator ConstructionRoutine(int constructionTime) {
		for (int i = constructionTime; i > 0; --i) {
			SetText (i + "s left");
			yield return new WaitForSeconds(1.0f);
		}
		building.FinishConstruction ();
	}

	public void SetText(string text) {
		Text textScript = GetComponentInChildren<Text> ();
		textScript.text = text;
	}
}
