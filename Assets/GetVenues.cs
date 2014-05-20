using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System;

public class GetVenues : MonoBehaviour {

	enum State{ 
		GETTING_VENUES,
		FOURSQUARE_NOT_CONNECTED,
		FOURSQUARE_CONNECT_PENDING,
		VENUES_RECEIVED,
		NO_VENUE_RECEIVED
	};

	int numVenues = 0;

	State state;

	// Use this for initialization
	void Start () {

		state = State.GETTING_VENUES;
		FetchVenues();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		string state_text = "";
		if (state == State.GETTING_VENUES) {
			state_text = "Getting venues...";	
		} else if(state == State.FOURSQUARE_NOT_CONNECTED) {
			state_text = "Foursquare not connected.";
			if(GUI.Button(new Rect(20,50,160,30),"Connect Foursquare")){
				state = State.FOURSQUARE_CONNECT_PENDING;
				Application.OpenURL ("https://squarewall.parseapp.com/foursquare_connect/"+ParseUser.CurrentUser.ObjectId);
			}
		} else if(state == State.FOURSQUARE_CONNECT_PENDING) {
			state_text = "Waiting for foursquare connection.";	
			if(GUI.Button(new Rect(20,50,160,30),"Check Again")){
				state = State.GETTING_VENUES;
				FetchVenues();
			}
		} else if(state == State.NO_VENUE_RECEIVED) {
			state_text = "Seems like connected foursquare account does not manage any venues.";	
			if(GUI.Button(new Rect(20,50,160,30),"Check Again")){
				state = State.GETTING_VENUES;
				FetchVenues();
			}
		}else if(state == State.VENUES_RECEIVED){
			state_text = "Downloading details for "+numVenues+" venue"+((numVenues>1)?"s":"");
		}
		GUI.Label (new Rect (20, 20, 500, 30), state_text);
	}

	void FetchVenues(){

		Debug.Log ("getting venues...");

		try{
			
			ParseCloud.CallFunctionAsync<string> ("venues", new Dictionary<string, object>())
				.ContinueWith(t =>{

					Debug.Log(t.Result);

					if (t.IsFaulted || t.IsCanceled){

						state = State.FOURSQUARE_NOT_CONNECTED;
						
					}else{


						Int32.TryParse(t.Result.Split(':')[1],out numVenues);
						if(numVenues<=0){
							state = State.NO_VENUE_RECEIVED;
						}else{
							state = State.VENUES_RECEIVED;
						}

					}
					
					
					
				});

		}catch(InvalidOperationException e){

			state = State.FOURSQUARE_NOT_CONNECTED;
			Debug.Log("error "+e.Message);
		}
		

	}
}
