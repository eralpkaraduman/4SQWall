using UnityEngine;
using System.Collections;

public class TestCheckinDisplay : MonoBehaviour {

	public CheckinSpawner checkinSpawner;


	public Transform alertParentTransform;

	public CheckinDisplay testInstance;
	public CheckinAlert testAlertInstance;

	// Use this for initialization
	void Start () {
		if (testInstance) {
			Destroy(testInstance.gameObject);
		}

		if (testAlertInstance) {
			Destroy(testAlertInstance.gameObject);
		}

		checkinSpawner.OnAlertQueueCompleted += alertsCompleted;

		checkinSpawner.reloadWheelWithParseObjects (null);

	}

	void alertsCompleted(){
		checkinSpawner.reloadWheelWithParseObjects (null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		if (GUI.Button (new Rect (10, 10, 120, 20), "Create Check in")) {
			//checkinSpawner.spawnWithJSONObject();
			//checkinSpawner.spawnWithJSONObject(null);



			checkinSpawner.alertNewCheckIn(null,alertParentTransform);

		}

		if (GUI.Button (new Rect (140, 10, 120, 20), "Reload")) {
			//checkinSpawner.spawnWithJSONObject();
			//checkinSpawner.spawnWithJSONObject(null);

			 

			checkinSpawner.reloadWheelWithParseObjects (null);
		};

	}
}
