using UnityEngine;
using System.Collections;

public class PreferencesButton : MonoBehaviour {

	public Texture preferencesButtonTexture = null;

	public static string levelBeforeOpeningPreferences = "Preferences";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
	
		if(GUI.Button(new Rect(Screen.width-45,5,40,40),preferencesButtonTexture)){

			PreferencesButton.levelBeforeOpeningPreferences = Application.loadedLevelName;
			Debug.Log("levelBeforeOpeningPreferences: "+PreferencesButton.levelBeforeOpeningPreferences);
			Application.LoadLevel("Preferences");

		}

	}
}
