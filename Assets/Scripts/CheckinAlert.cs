using UnityEngine;
using System.Collections;
using Parse;

public class CheckinAlert : MonoBehaviour {

	public Renderer profilePictureRenderer;
	public TextMesh userNameTextMesh;
	public Shader ProfilePicureShader;

	public delegate void onDestroyed();
	public onDestroyed OnDestroyed;

	float timer = 0.0f;
	bool dismissing = false;

	public Transform contentPivot;

	float easing = 8.0f;

	float positionYTarget = 0.0f;
	public bool dismissed = false;


	IEnumerator LoadPicture(string url) {
		
		Debug.Log ("loading.. "+url);
		
		WWW www = new WWW(url);
		yield return www;
		
		Debug.Log ("LOADED "+url);
		
		Material newMat = new Material (ProfilePicureShader);
		newMat.mainTexture = www.texture;
		profilePictureRenderer.material = newMat;
	}

	public void configure(ParseObject checkin){

		if (checkin == null) return;
		
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

			/*
			if(key == "cretedAtFoursquare"){
				fsCreated = (long)checkin["cretedAtFoursquare"];
			}
			
			if(key == "foursquareTimeZoneOffset"){
				fsOffset = (long)checkin["foursquareTimeZoneOffset"];
			}
			*/
		}
		
		//string fullName = firstName +" "+ lastName;
		string fullName = firstName;

		userNameTextMesh.text = fullName;
		
		string pictureURL = (string)checkin["userPhotoPrefix"] + "512x512" + (string)checkin["userPhotoSuffix"];


		Debug.Log ("fullName "+fullName);
		Debug.Log ("PURL "+pictureURL);

		StartCoroutine (LoadPicture (pictureURL));
		
	}

	// Use this for initialization
	void Start () {

		contentPivot.localPosition = new Vector3 (0, 70, 0);
	}
	
	// Update is called once per frame
	void Update () {

		if (dismissing == false) {

			timer += Time.deltaTime;
			if (timer > 3) {
				dismissing = true;

				positionYTarget = -70;
				
			}


		}else if(!dismissed){
		
			if(contentPivot.position.y - positionYTarget <= (easing/3)){
				dismissed = true;

				OnDestroyed();

				GameObject.Destroy(this.gameObject);

			}
		
		}

		Vector3 newPosition = contentPivot.position;

		newPosition.y += (positionYTarget - newPosition.y)/easing;
		contentPivot.position = newPosition;


	}
}
