using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Boomlagoon.JSON;
using Parse;

public class CheckinSpawner : MonoBehaviour {

	public GameObject wheel;

	public delegate void onAlertQueueCompleted();
	public onAlertQueueCompleted OnAlertQueueCompleted;

	Queue<ParseObject> checkinAlertQueue = new Queue<ParseObject>();



	int i = 0;

	float minRadius = 90;
	float minSpaceForCheckin = 100; 
	float scrollSpeed = 40.0f;

	private Transform alertParentTransform;


	//CheckinDisplay display;

	public CheckinDisplay checkinDisplayPrefab;
	public CheckinAlert checkinAlertPrefab;
	private CheckinAlert currentAlert = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		wheel.transform.Rotate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);

	}


	public void reloadWheelWithParseObjects(ArrayList checkinList){

		ClearOldWheel ();

		int numCheckIns = 5; // Test Mode

		if (checkinList != null) {
			numCheckIns = checkinList.Count;
		}

		float radius = (numCheckIns * minSpaceForCheckin) / (Mathf.PI * 2);
		if (radius < minRadius) radius = minRadius;

		Vector3 wheelPos = new Vector3 (0, 0, radius);
		wheel.transform.position = wheelPos;
		wheel.transform.localEulerAngles = new Vector3 (0, 180, 180);

		Camera.main.farClipPlane = radius*4.0f;

		scrollSpeed =  Screen.height / (2 * Mathf.PI * radius) * 50;

		for (int i=0; i<numCheckIns; i++) {

			int index = i;

			ParseObject checkin = null;
			if(checkinList != null){
				checkin = (ParseObject)checkinList[i];
			}

			spawnWithParseObject(checkin,index,numCheckIns,radius);

		}

	}

	public void alertNewCheckIn(ParseObject checkin,Transform parentTransform){

		if (checkin == null) { // test mode

			checkin = new ParseObject("Checkin");

		}

		alertParentTransform = parentTransform;
		checkinAlertQueue.Enqueue(checkin);
		alertFromQueue ();
		Debug.Log ("alert queue: "+checkinAlertQueue.Count);
	}

	public void alertFromQueue(){

		if (currentAlert != null) return;

		if (checkinAlertQueue.Count <= 0) {

			OnAlertQueueCompleted();

			return;
		}

		ParseObject checkin = checkinAlertQueue.Dequeue ();

		CheckinAlert alert = (CheckinAlert)GameObject.Instantiate (checkinAlertPrefab,new Vector3(0,0,0),Quaternion.identity);
		alert.configure (checkin);
		alert.OnDestroyed += onAlertDestroyed;
		alert.transform.parent = alertParentTransform;
		alert.transform.localPosition = Vector3.zero;
		alert.transform.localEulerAngles = Vector3.zero;
		currentAlert = alert;

	}

	public void onAlertDestroyed(){
		currentAlert = null;
		alertFromQueue ();
	}

	public void spawnWithParseObject(ParseObject checkin,int index, int total,float radius){

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

	}


	/*

	void QueueCheckinAlert(ParseObject checkin){

		checkinAlertQueue.Enqueue (checkin);

		AlertCheckins ();

	}

	void AlertCheckins(){
		if (currentCheckinAlert != null) {
			return;
		}

		ParseObject checkin = checkinAlertQueue.Dequeue ();
		if (checkin!=null) {
			
		}
	}

	*/
}
