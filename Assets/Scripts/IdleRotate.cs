using UnityEngine;
using System.Collections;

public class IdleRotate : MonoBehaviour {

	float speed = 50.0f;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.left * Time.deltaTime * speed);
		transform.Rotate(Vector3.down * Time.deltaTime * speed, Space.World);
	}
}
