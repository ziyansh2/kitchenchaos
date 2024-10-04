using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {


	[SerializeField] private PlatesCounter platesCounter;
	[SerializeField] private Transform counterTopPoint;
	[SerializeField] private Transform PlateVisualPrefab;

	private List<GameObject> plateVisualGameObjectList = new();

	private void Start() {
		platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
		platesCounter.OnPlatePicked += PlatesCounter_OnPlatePicked;
	}

	private void PlatesCounter_OnPlatePicked(object sender, System.EventArgs e) {
		GameObject lastPlate = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
		plateVisualGameObjectList.Remove(lastPlate);
		Destroy(lastPlate);
	}

	private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e) {
		Transform plateVisualTransform = Instantiate(PlateVisualPrefab, counterTopPoint);
		float plateOffsetY = .1f;
		plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);
		plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
	}

}
