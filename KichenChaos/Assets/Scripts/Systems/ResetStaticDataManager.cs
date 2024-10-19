using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour {

	private void Awake() {
		BaseCounter.ResetStaticData();
		TrashCounter.ResetStaticData();
		CuttingCounter.ResetStaticData();
	}


}
