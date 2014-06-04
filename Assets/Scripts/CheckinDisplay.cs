using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using Parse;

public class CheckinDisplay : MonoBehaviour {

	//public Material profilePictureMaterial;
	public Renderer profilePictureRenderer;
	public TextMesh userNameTextMesh;
	public TextMesh orderNumberTextMesh;
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

		userNameTextMesh.text = (string)checkin["userFirstName"];

		string pictureURL = (string)checkin["userPhotoPrefix"] + "512x512" + (string)checkin["userPhotoSuffix"];



		StartCoroutine (LoadPicture (pictureURL));



	}
	

	void Update () {


		float distance = Vector3.Distance (Camera.main.transform.position,this.transform.position);

		if (distance < 100) {

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
