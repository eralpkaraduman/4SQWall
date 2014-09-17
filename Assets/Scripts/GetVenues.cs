using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System;

public class GetVenues : MonoBehaviour {

	enum State{ 
		UPDATING_VENUES,
		FOURSQUARE_NOT_CONNECTED,
		FOURSQUARE_CONNECT_PENDING,
		VENUES_UPDATED,
		GETTING_VENUES,
		VENUES_RECEIVED,
		GETTING_VENUES_FAILED,
		NO_VENUE_RECEIVED
	};



	public static ParseObject selectedVenue = null; 

	int numVenues = 0;

	ICollection<ParseObject> venues;

	State state;

	void Start () {

		//state = State.UPDATING_VENUES;
		//FetchVenues();

		state = State.GETTING_VENUES;
		GetVenueDetails();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		GUI.color = Color.black;

		string state_text = "";
		if (state == State.UPDATING_VENUES) {
			state_text = "Updating venues...";	
		} else if(state == State.FOURSQUARE_NOT_CONNECTED) {
			state_text = "Foursquare not connected.";
			if(GUI.Button(new Rect(20,50,160,30),"Connect Foursquare")){
				state = State.FOURSQUARE_CONNECT_PENDING;
				Application.OpenURL ("https://squarewall.parseapp.com/foursquare_connect/"+ParseUser.CurrentUser.ObjectId);
			}
		} else if(state == State.FOURSQUARE_CONNECT_PENDING) {
			state_text = "Waiting for foursquare connection.";	
			if(GUI.Button(new Rect(20,50,160,30),"Check Again")){
				state = State.UPDATING_VENUES;
				FetchVenues();
			}
		} else if(state == State.NO_VENUE_RECEIVED) {
			state_text = "Seems like connected foursquare account does not manage any venues.";	
			if(GUI.Button(new Rect(20,50,160,30),"Check Again")){
				state = State.UPDATING_VENUES;
				FetchVenues();
			}
		}else if(state == State.VENUES_UPDATED){

			state = State.GETTING_VENUES;
			GetVenueDetails();


		}else if(state == State.GETTING_VENUES){

			if(numVenues>1){
				state_text = "Getting venue details for "+numVenues+" venue"+((numVenues>1)?"s":"")+"...";
			}else{
				state_text = "Getting venue details ...";
			}

		}else if(state == State.VENUES_RECEIVED){

			if(venues == null || venues.Count<=0){
				state_text = "No managed venues found";

				if(GUI.Button(new Rect(20,50,160,30),"Update Managed Venues")){
					state = State.UPDATING_VENUES;
					FetchVenues();
				}

			}else{

				state_text = "Received "+numVenues+" venue"+((numVenues>1)?"s":"")+", select one.";

				var offset = 50;
				var margin = 10;
				var h_margin = 20;
				var venueButtonHeight = 50;

				IList venueList = venues as IList;
				GUI.color = Color.white;
				for(var i = 0; i<numVenues; i++){

					ParseObject venue = (ParseObject)venueList[i];
					var venueName = venue.Get<string>("name");
					var venueAddress = venue.Get<string>("address");
					var venueText = venueName+" "+venueAddress;



					if(GUI.Button(new Rect(h_margin,offset,Screen.width-h_margin*2,venueButtonHeight),venueText)){
						SelectVenue(venue);
					}
					offset += (margin+venueButtonHeight);
				}
			}

		}

		GUI.color = Color.black;

		// print
		GUI.Label (new Rect (20, 20, 500, 30), state_text);


	}

	void SelectVenue(ParseObject venue){
		selectedVenue = venue;

		Application.LoadLevel ("Wall");
	}

	void FetchVenues(){

		try{
			
			ParseCloud.CallFunctionAsync<string> ("updateVenues", new Dictionary<string, object>())
				.ContinueWith(t =>{

					Debug.Log(t.Result);

					if (t.IsFaulted || t.IsCanceled){

						state = State.FOURSQUARE_NOT_CONNECTED;
						
					}else{

						if(t.Result == "ERROR"){

							state = State.FOURSQUARE_NOT_CONNECTED;
							Debug.Log("Fetch venues error");

						}else{

							Debug.Log("Fetch venues success");

							Int32.TryParse(t.Result.Split(':')[1],out numVenues);
							if(numVenues<=0){
								state = State.NO_VENUE_RECEIVED;
							}else{
								state = State.VENUES_UPDATED;
							}
						}





					}
					
					
					
				});

		}catch(InvalidOperationException e){
			Debug.Log("NEFLGSK2");
			state = State.FOURSQUARE_NOT_CONNECTED;
			Debug.Log("error "+e.Message);
		}
		

	}

	void GetVenueDetails(){

		ParseUser.CurrentUser.FetchAsync().ContinueWith(t =>{

			var relation = ParseUser.CurrentUser.GetRelation<ParseObject>("venues");
			var query = relation.Query;


			query.FindAsync().ContinueWith(t2 =>{

				if (t.IsFaulted || t.IsCanceled){
					state = State.GETTING_VENUES_FAILED;
				}else{
					venues = (ICollection<ParseObject>)t2.Result;
					numVenues = venues.Count;
					state = State.VENUES_RECEIVED;
				}
			});

		});


	}
}
