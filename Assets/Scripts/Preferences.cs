using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;
using Boomlagoon.JSON;

public class Preferences : MonoBehaviour {

	enum FoursquareAccountStatus{ 
		GETTING_ACCOUNT_DETAILS,
		GETTING_ACCOUNT_DETAILS_FAILED,
		NOT_CONNECTED,
		CONNECTED,
		CONNECTION_PENDING
	};

	JSONObject foursquareUserData = null;
	string foursquareUserDataGetError = null;

	FoursquareAccountStatus foursquareAccountStatus;

	// Use this for initialization
	void Start () {


		GetConnectedFoursquareUser ();
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

		if (foursquareAccountStatus == FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS) {
			GUI.enabled = false;
		}

		if (GUI.Button (new Rect (20, (offset += inc), Screen.width - 40, 40),FoursquareAccountStatusString())) {
			
			if (foursquareAccountStatus == FoursquareAccountStatus.CONNECTED) {

				DisconnectFoursquare();

			}else if(foursquareAccountStatus == FoursquareAccountStatus.CONNECTION_PENDING){

				GetConnectedFoursquareUser();

			}else{

				ConnectFoursquare();

			}
		}

		GUI.enabled = true;

		if (GUI.Button (new Rect (20, (offset += inc), Screen.width - 40, 40),"Cancel")) {

			Application.LoadLevel(PreferencesButton.levelBeforeOpeningPreferences);
		}

	}

	void DisconnectFoursquare(){
	
		foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS;

		Parse.ParseUser.CurrentUser["foursquareAccessToken"] = null;
		Parse.ParseUser.CurrentUser.SaveAsync().ContinueWith(t =>{

			if (t.IsFaulted || t.IsCanceled){

				foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS_FAILED;

			}else{
			
				foursquareAccountStatus = FoursquareAccountStatus.NOT_CONNECTED;
			}

		});

	}

	void ConnectFoursquare(){

		foursquareAccountStatus = FoursquareAccountStatus.CONNECTION_PENDING;
		Application.OpenURL ("https://squarewall.parseapp.com/foursquare_connect/"+ParseUser.CurrentUser.ObjectId);

	}

	string FoursquareAccountStatusString(){

		if (foursquareAccountStatus == FoursquareAccountStatus.CONNECTION_PENDING) {
		
			return "Waiting For Foursquare Connection, Click When Done";	
		}

		if (foursquareAccountStatus == FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS) {
			return "Checking Connected Foursquare Account: ...";
		}

		if (foursquareAccountStatus == FoursquareAccountStatus.CONNECTED) {

			string firstName = foursquareUserData.GetString("firstName");
			//string lastName = foursquareUserData.GetString("lastName");
			string email = foursquareUserData.GetObject("contact").GetString("email");

			return "! Disconnect Foursquare Account: "+ firstName+" "+email+" !";
		}

		if (foursquareAccountStatus == FoursquareAccountStatus.NOT_CONNECTED) {
		
			return "No Foursquare Account Connected, Click To Connect.";
		}

		if (foursquareAccountStatus == FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS_FAILED) {
			
			return "Foursquare Account May Be Expired, Click To Connect Again.";
		}

		return "Foursquare Account Status Unknown";
	
	}

	void GetConnectedFoursquareUser(){

		Debug.Log("getting connectedFoursquareUser...");
		foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS;
		foursquareUserData = null;
		foursquareUserDataGetError = null;

		try{
			
			ParseCloud.CallFunctionAsync<string> ("connectedFoursquareUser", new Dictionary<string, object>())
				.ContinueWith(t =>{
					
					if (t.IsFaulted || t.IsCanceled){

						Debug.Log("connectedFoursquareUser fauled or cancelled");
						foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS_FAILED;
						//state = State.FOURSQUARE_NOT_CONNECTED;
						
					}else{

						string jsonString = t.Result;
						JSONObject json = JSONObject.Parse(jsonString);
						JSONObject user = json.GetObject("user");
						if(user!=null){
							foursquareUserData = user;
							foursquareAccountStatus = FoursquareAccountStatus.CONNECTED;
						}else{

							foursquareAccountStatus = FoursquareAccountStatus.NOT_CONNECTED;
							foursquareUserDataGetError = json.GetString("error");
						}


						//Debug.Log("firstName "+user.GetString("firstName"));

					}
					
					
					
				});
			
		}catch(InvalidOperationException e){
			foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS_FAILED;
			Debug.Log("error "+e.Message);
		}
		
		
	}

}
