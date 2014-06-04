using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using Parse;



public class CheckinSpawner : MonoBehaviour {

	public GameObject wheel;

	int i = 0;

	float minRadius = 90;
	float minSpaceForCheckin = 80; 
	float scrollSpeed = 50.0f;


	//CheckinDisplay display;

	public CheckinDisplay checkinDisplayPrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		wheel.transform.Rotate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);

	}


	public void reloadWheelWithParseObjects(ArrayList checkinList){

		ClearOldWheel ();

		//int numCheckIns = 44;
		int numCheckIns = checkinList.Count;

		float radius = (numCheckIns * minSpaceForCheckin) / (Mathf.PI * 2);
		if (radius < minRadius) radius = minRadius;

		Vector3 wheelPos = new Vector3 (0, 0, radius);
		wheel.transform.position = wheelPos;
		wheel.transform.localEulerAngles = new Vector3 (0, 180, 180);

		Camera.main.farClipPlane = radius*4.0f;

		scrollSpeed =  Screen.height / (2 * Mathf.PI * radius) * 50;

		for (int i=0; i<numCheckIns; i++) {

			int index = i;

			ParseObject checkin = (ParseObject)checkinList[i];

			spawnWithParseObject(checkin,index,numCheckIns,radius);

		}

	}

				


	public void spawnWithParseObject(ParseObject checkin,int index, int total,float radius){
		//CheckinDisplay checkin = (CheckinDisplay)Instantiate (checkinDisplayPrefab.transform);

		CheckinDisplay display = (CheckinDisplay)GameObject.Instantiate (checkinDisplayPrefab,new Vector3(0,0,0),Quaternion.identity);

		display.configureWithParseObject(index,checkin);

		display.transform.parent = wheel.transform;

		display.transform.localPosition = GetWheelPosition(index,total,radius);
	}

	Vector3 GetWheelPosition(int index, int total, float radius){


		float i = (index * 1.0f)/total;
		float angle = i * Mathf.PI * 2;

		float x = Mathf.Sin (angle) * radius;
		float y = Mathf.Cos (angle) * radius;

		Vector3 pos = new Vector3 (0,x,y);

		return pos;
	}

	void ClearOldWheel (){

		GameObject.Destroy (wheel);
		wheel = new GameObject ();
		wheel.transform.parent = this.transform;

		/*
		while (wheel.transform.childCount>0) {
			GameObject.Destroy(wheel.transform.GetChild(0).gameObject);
		}
		*/
	}
}
