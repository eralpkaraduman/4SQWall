using UnityEngine;
using System.Collections;
using SocketIOClient;
using System;
using SimpleJson;
using System.Reflection;
using System.Collections.Generic;
using Parse;

public class SocketConnection : MonoBehaviour {

	public string socketServerRoot = "http://127.0.0.1:3000";
	protected static string subscription_token = null;
	ParseObject selectedVenue;

	public enum SocketStatus{
		SUBSCRIBING,
		DISCONNECTED,
		CONNECTING,
		FETCHING_USER

	}

	public SocketStatus socketStatus;

	// Use this for initialization
	void Start () {

		socketStatus = SocketStatus.DISCONNECTED;

		selectedVenue = GetVenues.selectedVenue;

		Connect ();
	}

	void OnGUI(){
	
		GUI.Label (new Rect ( 10, Screen.height - 30, 200, 30), "SOCKET: " + socketStatus);

	}

	// Update is called once per frame
	void Update () {
	
	}

	void Connect(){

		if (subscription_token == null) {
		
			Subscribe();

		}

		socketStatus = SocketStatus.CONNECTING;


		/*
		client = new Client (socketServerRoot+"?subscription_token="+subscription_token);
		
		//Client client = new Client ("http://localhost:3000");

		
		
		client.Opened += SocketOpened;
		client.Message += SocketMessage;
		client.SocketConnectionClosed += SocketConnectionClosed;
		client.Error +=SocketError;
		
		
		Debug.Log ("connecting..");
		client.Connect ();

		*/
	}

	void Subscribe(){
		socketStatus = SocketStatus.FETCHING_USER;

		//Debug.Log ("venue id " + selectedVenue ["fourid"]); 

		ParseUser user = ParseUser.CurrentUser;

		user.FetchAsync ().ContinueWith (t => {

			socketStatus = SocketStatus.SUBSCRIBING;

			var relation = user.GetRelation<ParseObject> ("subscribedVenue");
			relation.Add(selectedVenue);
			user.SaveAsync().ContinueWith(t2=>{

				subscription_token = (string)user["subscriptionToken"];

				Connect();


				

			});

		});





		/*
		Parse.ParseUser.CurrentUser["subscribedVenueId"] = null;
		Parse.ParseUser.CurrentUser.SaveAsync().ContinueWith(t =>{
			
			if (t.IsFaulted || t.IsCanceled){
				
				foursquareAccountStatus = FoursquareAccountStatus.GETTING_ACCOUNT_DETAILS_FAILED;
				
			}else{
				
				foursquareAccountStatus = FoursquareAccountStatus.NOT_CONNECTED;
			}
			
		});

		*/
	}
}
