using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building {

	private int buildingTypeId;
	private GameObject buildingObject;
	private bool inConstruction = false;
	private int x;
	private int y;
	private int health = 10;

	public Building(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public bool hasBuilding() {
		return buildingObject != null;
	}

	public void InitiateConstruction(int buildingTypeId, GameObject buildingType, int constructionTime) {
		this.health = 10;
		this.buildingTypeId = buildingTypeId;

		buildingObject = GameObject.Instantiate (buildingType, new Vector3 (x * 20 + 10, 0, y * 20 + 10), Quaternion.identity);
		BuildingObject buildingScript = buildingObject.AddComponent<BuildingObject> ();
		buildingScript.Init (this);

		Renderer[] renderers = buildingObject.GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.material.color = new Color (0.8f, 0.3f, 0.3f);
		}
			

		GameController controller = GameObject.Find ("GameController").GetComponent<GameController> ();

		GameObject buildingText = GameObject.Instantiate (controller.textOverBuildings);
		buildingText.transform.SetParent (buildingObject.transform, false);

		if (constructionTime != 0) {
			inConstruction = true;
			buildingScript.InitConstruction (constructionTime);
		} else
			FinishConstruction ();
	}

	public void FinishConstruction() {
		if (buildingObject == null) {
			return;
		}

		inConstruction = false;

		Renderer[] renderers = buildingObject.GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.material.color = Color.white;
		}

		SetText (health + "hp");
	}

	public void SetText(string text) {
		if (buildingObject == null)
			return;
		buildingObject.GetComponent<BuildingObject> ().SetText (text);
	}

	public bool IsInConstruction() {
		return inConstruction;
	}

	public int GetBuildingType() {
		return buildingTypeId;
	}

	public void Hit() {
		if (buildingObject != null) {
			health -= 1;
			SetText (health + "hp");
			if (health == 0) {
				GameObject.Destroy (buildingObject);
				buildingObject = null;
				inConstruction = false;
			}
		}
	}
}
