using UnityEngine;
using System.Collections;

public class TestCheckinDisplay : MonoBehaviour {

	public CheckinSpawner checkinSpawner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		if (GUI.Button (new Rect (20, 20, 200, 50), "Create Check in")) {
			
		};
	}
}
