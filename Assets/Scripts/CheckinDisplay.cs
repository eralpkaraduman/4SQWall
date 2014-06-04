using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using Parse;
using System;

public class CheckinDisplay : MonoBehaviour {

	//public Material profilePictureMaterial;
	public Renderer profilePictureRenderer;
	public TextMesh userNameTextMesh;
	public TextMesh orderNumberTextMesh;
	public TextMesh timeTextMesh;
	public Shader ProfilePicureShader;

	public Color nameNearColor;
	public Color nameFarColor;

	public MeshRenderer textBackgroundMeshRenderer;

	// Use this for initialization
	void Start () {
		
	}

	IEnumerator LoadPicture(string url) {

		Debug.Log ("loading.. "+url);

		WWW www = new WWW(url);
		yield return www;

		Debug.Log ("LOADED "+url);

		Material newMat = new Material (ProfilePicureShader);
		newMat.mainTexture = www.texture;
		profilePictureRenderer.material = newMat;
	}

	public void configureWithParseObject(int index,ParseObject checkin){

		orderNumberTextMesh.text = "" + (index+1);

		string firstName = "";
		string lastName = "";
		long fsCreated = 0;
		long fsOffset = 0;

		foreach (string key in checkin.Keys) {
			//Debug.Log("k "+key);

			if(key == "userFirstName"){
				firstName = (string)checkin[key];
			}

			if(key == "userlastName"){
				lastName = (string)checkin[key];
			}

			if(key == "createdAtFoursquare"){
				fsCreated = (long)checkin["createdAtFoursquare"];
			}

			if(key == "foursquareTimeZoneOffset"){
				fsOffset = (long)checkin["foursquareTimeZoneOffset"];
			}
		}

		string fullName = firstName +" "+ lastName;


		userNameTextMesh.text = fullName;

		printTime (fsCreated,fsOffset);

		string pictureURL = (string)checkin["userPhotoPrefix"] + "512x512" + (string)checkin["userPhotoSuffix"];

		StartCoroutine (LoadPicture (pictureURL));

	}

	void printTime(long timestamp,long offset){
		timestamp = timestamp - offset;

		DateTime date = Wall.DateTimeFromUnixTimeStamp (timestamp);
		string formattedDate = date.ToString("HH:mm:ss");
		timeTextMesh.text = formattedDate;
	}

	void Update () {


		float distance = Vector3.Distance (Camera.main.transform.position,this.transform.position);

		if (distance < 120) {

			userNameTextMesh.color = nameNearColor;

			transform.LookAt (this.transform.parent);

			textBackgroundMeshRenderer.enabled = true;

		} else {


			userNameTextMesh.color = nameFarColor;

			Vector3 pos = transform.parent.position;
			pos.z *= 10;

			transform.LookAt(pos);

			textBackgroundMeshRenderer.enabled = false;
		}





	}
}
