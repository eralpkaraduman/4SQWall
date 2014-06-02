using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class CheckinDisplay : MonoBehaviour {

	public Material profilePictureMaterial;
	public TextMesh userNameTextMesh;
	public TextMesh orderNumberTextMesh;

	public Color nameNearColor;
	public Color nameFarColor;

	public MeshRenderer textBackgroundMeshRenderer;

	// Use this for initialization
	void Start () {
		
	}

	public void configureWithJSONObject(int index,JSONObject jsonObject){

		orderNumberTextMesh.text = "" + (index+1);
	
	}
	
	// Update is called once per frame
	void Update () {


		float distance = Vector3.Distance (Camera.main.transform.position,this.transform.position);

		//Debug.Log (distance);



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
