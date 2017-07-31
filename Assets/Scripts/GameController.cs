using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject textOverBuildings;

	public GameObject buildingRocket;

	public GameObject buildingMetalsExtractor;
	public GameObject buildingLOXExtractor;
	public GameObject buildingSolarPanels;
	public GameObject buildingMissileLauncher;

	public GameObject asteroidGroup;

	public GameObject selectionObject;

	public int currentTileX = 0;
	public int currentTileY = 0;
	public Building[] buildings = new Building[5 * 5];

	public int selectedToBuild = -1;
	public string[] resourceName = {
		"Metal",
		"LOX+RP",
		"Energy"
	};

	public int[,] costPerBuilding = new int[,] {
		{100, 0, 0}, // metals extractor
		{200, 20, 0}, // lox+rp extractor
		{50, 0, 0}, // solar panels
		{400, 0, 0}, // missile launcher
	};

	public int[,] consumptionPerBuilding = new int[,] {
		{0, 10, 60}, // metals extractor
		{0, 0, 50}, // lox+rp extractor
		{0, 0, 0}, // solar panels
		{0, 0, 10}, // missile launcher
	};

	public int[,] productionPerBuilding = new int[,] {
		{20, 0, 0}, // metals extractor
		{0, 30, 0}, // lox+rp extractor
		{0, 0, 60}, // solar panels
		{0, 0, 0}, // missile launcher
	};

	public string[] buildingTooltip = new string[4];

	public int[] costMissile = new int[] { 20, 30, 40 };

	public float[] resources = new float[3];
	public int asteroidSpawnTime;

	public GameObject rocketInfoText;
	public GameObject launchRocketButton;
	public GameObject endText;
	public GameObject timeStranded;

	private bool isFinishing = false;
	private float timeStart;

	void Start () {
		timeStart = Time.time;
		for (int i = 0; i < 25; i++) {
			buildings [i] = new Building (i % 5 - 2, i / 5 - 2);
		}
		buildings [12].InitiateConstruction (-1, buildingRocket, 0);
		buildings [12].SetText ("");

		int buildingId = 0;
		for(int i = 0; i < 4; i++) {
			GameObject buildingSelect = GameObject.Find ("Building" + i);
			buildingSelect.transform.Find("Cost0").gameObject.GetComponent<Text>().text = "" + costPerBuilding[buildingId, 0];
			buildingSelect.transform.Find("Cost1").gameObject.GetComponent<Text>().text = "" + costPerBuilding[buildingId, 1];
			buildingSelect.transform.Find("Cost2").gameObject.GetComponent<Text>().text = "" + costPerBuilding[buildingId, 2];
			buildingId++;
		}

		resources [0] = 300; // metals
		resources [1] = 200; // lox + rp
		resources [2] = 1000; // energy
		UpdateResources ();

		for (int i = 0; i < 4; i++) {
			string produce = "";
			string consume = "";
			for (int resource = 0; resource < 3; resource++) {
				if (productionPerBuilding [i, resource] != 0) {
					produce += productionPerBuilding [i, resource] + " " + resourceName [resource] + " ";
				}
				if (consumptionPerBuilding [i, resource] != 0) {
					consume += consumptionPerBuilding [i, resource] + " " + resourceName [resource] + " ";
				}
			}


			buildingTooltip [i] = (produce.Length == 0 ? "" : ("| Produces " + produce)) + (consume.Length == 0 ? "" : ("| Consumes " + consume));
			if (i == 3)
				buildingTooltip [i] += "| Missile cost " + costMissile [0] + " " + resourceName [0] + " " + costMissile [1] + " " + resourceName [1] + " " + costMissile [2] + " " + resourceName [2];
		}

		UpdateAsteroidTime ();
	}

	public string PrettyTime(int time) {
		string timeReturn = "";
		if (time / 3600 >= 1) {
			timeReturn += (time / 3600) + "h";
			time -= (int)Mathf.Floor(time / 3600) * 3600;
		}

		if (time / 60 >= 1)
			timeReturn += (time / 60) + "m";

		timeReturn += (time % 60) + "s";
		return timeReturn;
	}

	void Update () {
		if (isFinishing)
			return;

		timeStranded.GetComponent<Text>().text = "Time stranded: " + PrettyTime((int) Mathf.Floor(Time.time - timeStart));
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast (ray, out hit)) {
			Transform objectHit = hit.transform;
			float x = hit.point.x + Mathf.Sign(hit.point.x) * (-((Mathf.Sign(hit.point.x) * hit.point.x) % 20) + 10);
			float y = hit.point.z + Mathf.Sign(hit.point.z) * (-((Mathf.Sign(hit.point.z) * hit.point.z) % 20) + 10);
			int px = (int)(hit.point.x / 20);
			int py = (int)(hit.point.z / 20);
			if (hit.point.x < 0)
				px -= 1;
			if (hit.point.z < 0)
				py -= 1;

			if ((currentTileX != px || currentTileY != py) && IsValidTile(px, py)) {
				selectionObject.transform.position = new Vector3 (x, 0, y);
				EnterTile (px, py);
			}

			CheckTileInput ();
		}

		if (Input.GetKeyDown (KeyCode.Alpha1))
			SelectToBuild(0);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			SelectToBuild(1);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			SelectToBuild(2);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			SelectToBuild(3);
		
		if (Input.GetButtonDown ("Fire2"))
			SelectToBuild(-1);

		for (int i = 0; i < 25; ++i) {
			Building building = buildings [i];

			if (building.hasBuilding() && !building.IsInConstruction () && building.GetBuildingType() != -1) {
				int type = building.GetBuildingType ();

				float c0 = consumptionPerBuilding [type, 0] * Time.deltaTime;
				float c1 = consumptionPerBuilding [type, 1] * Time.deltaTime;
				float c2 = consumptionPerBuilding [type, 2] * Time.deltaTime;

				if (resources [0] - c0 >= 0 && resources [1] - c1 >= 0 && resources [2] - c2 >= 0) {
					resources [0] -= c0;
					resources [1] -= c1;
					resources [2] -= c2;

					resources [0] += productionPerBuilding[type, 0] * Time.deltaTime;
					resources [1] += productionPerBuilding[type, 1] * Time.deltaTime;
					resources [2] += productionPerBuilding[type, 2] * Time.deltaTime;
				}
				UpdateResources ();
			}
		}
	}

	void FixedUpdate() {
		if (asteroidSpawnTime <= Time.time) {
			GameObject asteroidGroupObject = Instantiate(asteroidGroup, new Vector3(400 * Random.value - 200, 100 * Random.value + 100, Random.value * 100), Quaternion.identity);
			AsteroidGroup asteroidGroupScript = asteroidGroupObject.GetComponent<AsteroidGroup> ();
			int x, y;
			do {
				x = Random.Range (0, 5);
				y = Random.Range (0, 5);
			} while(x == 2 && y == 2);

			asteroidGroupScript.SetTarget ((x - 2) * 20 + 10, (y - 2) * 20 + 10, buildings [x + y*5]);

			UpdateAsteroidTime ();
		}
	}

	public void SelectToBuild(int i) {
		selectedToBuild = i;
		RectTransform transform = GameObject.Find ("BuildingSelected").GetComponent<RectTransform> ();
		if (i >= 0) {
			transform.position = new Vector3 (20 + (64 + 10) * i, 20, 0);
			GameObject.Find("BuildingTooltip").GetComponent<Text>().text = buildingTooltip[i];
		} else {
			transform.position = new Vector3 (-1000.0f, 20.0f, 0.0f);
			GameObject.Find("BuildingTooltip").GetComponent<Text>().text = "";
		}
		UpdateSelectionColor ();
	}

	public bool IsInBuildMode() {
		return selectedToBuild != -1;
	}

	public void UpdateSelectionColor() {
		if (IsInBuildMode ()) {
			if (!IsTileBuildable (currentTileX, currentTileY)) {
				selectionObject.GetComponent<Renderer> ().material.color = new Color (1.0f, 0.0f, 0.0f, 0.50f);
			} else {
				selectionObject.GetComponent<Renderer> ().material.color = new Color (0.0f, 1.0f, 0.0f, 0.50f);
			}
		} else {
			selectionObject.GetComponent<Renderer> ().material.color = new Color (0.0f, 1.0f, 1.0f, 0.50f);
		}
	}

	public void EnterTile(int x, int y) {
		//Debug.Log (px + " " + py);

		currentTileX = x;
		currentTileY = y;
		UpdateSelectionColor ();
	}

	public bool IsValidTile(int x, int y) {
		return x >= -2 && x <= 2 && y >= -2 && y <= 2;
	}

	public Building GetTileBuilding(int x, int y) {
		if (IsValidTile(x, y)) {
			return buildings[(x + 2) + (y + 2) * 5];
		}
		return null;
	}

	public bool IsTileBuildable(int x, int y) {
		Building building = GetTileBuilding (x, y);
		return building != null && !building.hasBuilding();
	}

	public GameObject GetSelectedToBuildGameObject() {
		switch (selectedToBuild) {
		case 0:
			return buildingMetalsExtractor;
		case 1:
			return buildingLOXExtractor;
		case 2:
			return buildingSolarPanels;
		case 3:
			return buildingMissileLauncher;
		default:
			return null;
		}
	}

	public bool PurchaseMissile() {
		if (resources [0] >= costMissile [0] && resources [1] >= costMissile [1] && resources [2] >= costMissile [2]) {
			resources [0] -= costMissile [0];
			resources [1] -= costMissile [1];
			resources [2] -= costMissile [2];
			return true;
		}
		return false;
	}

	public bool HasResourcesToBuild() {
		if (selectedToBuild < 0)
			return false;
		return resources [0] >= costPerBuilding[selectedToBuild, 0] && resources [1] >= costPerBuilding[selectedToBuild, 1] && resources [2] >= costPerBuilding[selectedToBuild, 2];
	}

	public void SubtractResources() {
		if (selectedToBuild >= 0) {
			resources [0] -= costPerBuilding[selectedToBuild, 0];
			resources [1] -= costPerBuilding[selectedToBuild, 1];
			resources [2] -= costPerBuilding[selectedToBuild, 2];
			UpdateResources ();
		}
	}

	public void UpdateResources() {
		for (int i = 0; i < 3; ++i) {
			Text text = GameObject.Find ("Resource" + i).GetComponent<Text>();
			text.text = resourceName[i] + ": " + Mathf.Floor(resources [i]);
		}

		if (resources [0] >= 5000 && resources [1] >= 15000 && resources [2] >= 10000) {
			launchRocketButton.SetActive (true);
		} else {
			launchRocketButton.SetActive (false);
		}
	}

	public void CheckTileInput() {
		if (IsTileBuildable(currentTileX, currentTileY) && selectedToBuild != -1) {
			Building building = GetTileBuilding (currentTileX, currentTileY);

			if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject()) {
				if (!building.hasBuilding () && HasResourcesToBuild()) {
					building.InitiateConstruction (selectedToBuild, GetSelectedToBuildGameObject(), 5);
					UpdateSelectionColor ();
					SubtractResources ();
				}
			}
		}
	}

	public void UpdateAsteroidTime() {
		asteroidSpawnTime = (int)Time.time + 5 * Random.Range (1, 2); 
	}

	public void ToggleInfoText() {
		rocketInfoText.SetActive(!rocketInfoText.activeInHierarchy);
	}

	public void EndGame() {
		isFinishing = true;
		GameObject.Find ("Canvas").SetActive (false);
		StartCoroutine (SwitchEndGame ());
	}

	IEnumerator SwitchEndGame() {
		yield return new WaitForSeconds (6.0f);
		endText.SetActive (true);
	}

	public void RestartGame() {
		SceneManager.LoadScene ("MainMenu");
	}
}
