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

	public void configure(ParseObject checkin){
		
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
