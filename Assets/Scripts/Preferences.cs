using UnityEngine;
using System.Collections;

public class Preferences : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		var offset = 20;
		var inc = 50;
		
		if (GUI.Button (new Rect (20, (offset += inc), Screen.width - 40, 40),"Sign Out")) {
		
			Parse.ParseUser.LogOut();
			Application.LoadLevel("Intro");
		}

	}
}
