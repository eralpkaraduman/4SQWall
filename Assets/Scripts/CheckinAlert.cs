using UnityEngine;
using System.Collections;
using Parse;
using System;

public class CheckinAlert : MonoBehaviour {

	public Renderer profilePictureRenderer;
	public TextMesh userNameTextMesh;
	public Shader ProfilePicureShader;

	public float duration = 5.2f;

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

		if ((bool)checkin ["isDummy"] == true) return;

		if (checkin == null) return;
		
		string firstName = "";
		string lastName = "";
		long fsCreated = 0;
		long fsOffset = 0;
		
		foreach (string key in checkin.Keys) {

			if(key == "userFirstName"){
				firstName = (string)checkin[key];
			}
			
			if(key == "userlastName"){
				lastName = (string)checkin[key];
			}

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
			if (timer > duration) {
				dismissing = true;

				positionYTarget = -70;
				
			}


		}else if(!dismissed){
		
			if(contentPivot.position.y - positionYTarget <= (easing/3)){
				dismissed = true;

				try{
					OnDestroyed();
				}catch(NullReferenceException e){
				
				}

				GameObject.Destroy(this.gameObject);

			}
		
		}

		Vector3 newPosition = contentPivot.position;

		newPosition.y += (positionYTarget - newPosition.y)/easing;
		contentPivot.position = newPosition;


	}
}
