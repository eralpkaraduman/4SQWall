using UnityEngine;
using System.Collections;
using SocketIOClient;
using SocketIOClient.Messages;
using System;
using SimpleJson;
using System.Reflection;
using System.Collections.Generic;
using Parse;
using Boomlagoon.JSON;
using System.Text;

public class SocketConnection : MonoBehaviour {

	public string socketServerRoot = "http://54.204.2.155:3456";
	//public string socketServerRoot = "http://127.0.0.1:3000";
	protected static string subscription_token = null;
	ParseObject selectedVenue;

	Client client = null;

	public delegate void OnNewCheckinReceived(JSONObject jsonCheckin);
	public OnNewCheckinReceived checkInReceived;  

	public enum SocketStatus{
		SUBSCRIBING,
		DISCONNECTED,
		CONNECTING,
		CONNECTED,
		CONNECTED_REGISTERING_SESSION,
		FETCHING_USER,
		ERROR,
		CONNECTED_SUBSCRIBED,
		CONNECTED_COULD_NOT_SUBSCRIBE
	}

	public SocketStatus socketStatus;

	// Use this for initialization
	void Start () {

		socketStatus = SocketStatus.DISCONNECTED;
	}

	public void Begin(){
		if (client!=null) {
			client.Close ();
		}
		
		socketStatus = SocketStatus.DISCONNECTED;
		
		selectedVenue = GetVenues.selectedVenue;
		
		Connect ();

	}

	void OnGUI(){
	
		GUI.color = Color.black;

		GUI.Label (new Rect ( 10, Screen.height - 30, 400, 30), "SOCKET: " + socketStatus);

	}

	// Update is called once per frame
	void Update () {
	
	}

	void Connect(){

		if (client!=null) {
			client.Close ();
		}

		if (subscription_token == null) {
			Subscribe();
			return;
		}

		socketStatus = SocketStatus.CONNECTING;

		Debug.Log ("connecting...");


		client = new Client (socketServerRoot+"?subscription_token="+subscription_token);
		client.Opened += SocketOpened;
		client.SocketConnectionClosed += SocketConnectionClosed;
		client.Error +=SocketError;
		client.Message += SocketMessage;

		client.Connect ();
	}

	private void SocketMessage (object sender, MessageEventArgs e) {
		
		Debug.Log ("socket message received");

		if ( e!= null && e.Message.Event == "message") {

			string rawMessage = e.Message.MessageText;

			Debug.Log("raw "+rawMessage);

			JSONObject jsonMessage = null;

			try{
				jsonMessage = JSONObject.Parse(rawMessage);
			}catch(InvalidOperationException exception){

				jsonMessage = null;
				Debug.Log("could not parse response json");
			}

			if(jsonMessage!=null){
				Debug.Log("parsed json message");
				JSONArray argArray = jsonMessage.GetArray("args");
				JSONValue messageJSON = argArray[0];
				JSONObject messageObject = JSONObject.Parse(messageJSON.ToString());
				HandleSocketMessage(messageObject);

			}
				
		}

	}

	void HandleSocketMessage(JSONObject messageObject){
	
		string method = messageObject.GetString ("method");

		if (method == "register_session") {

			HandleRegisterSession(messageObject);

		}else if(method == "check_in_received") {

			HandleCheckInReceived(messageObject);
		}

	}


	void HandleRegisterSession(JSONObject messageObject){

		Boolean subscribed = messageObject.GetBoolean("subscribed");
		
		if(subscribed == true){
			socketStatus = SocketStatus.CONNECTED_SUBSCRIBED;
		}else{
			socketStatus = SocketStatus.CONNECTED_COULD_NOT_SUBSCRIBE;
		}

	}

	void HandleCheckInReceived(JSONObject messageObject){
		
		//Boolean subscribed = messageObject.GetBoolean("subscribed");
		
		Debug.Log("HandleCheckInReceived");

		Debug.Log ("type: "+messageObject.GetString("type"));


		if(messageObject.GetString("type") == "checkin"){

			checkInReceived(messageObject);

		}else{
			Debug.Log("received checkin has no type = checkin");
		}

		/*
		if(subscribed == true){
			socketStatus = SocketStatus.CONNECTED_SUBSCRIBED;
		}else{
			socketStatus = SocketStatus.CONNECTED_COULD_NOT_SUBSCRIBE;
		}
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

	}

	private void SocketOpened(object sender, EventArgs e) {


		Debug.Log ("SocketOpened");

		socketStatus = SocketStatus.CONNECTED_REGISTERING_SESSION;

		/*

		

		
		//string jsonData = SimpleJson.SimpleJson.SerializeObject (data);

		Debug.Log ("sending..");
		client.Send (new JSONMessage (data,0,"register_session"));
		//client.Send (jsonData);

		*/

		/*


		JSONObject data = new JSONObject ();
		data.Add ("method","register_session");
		data.Add ("user_id",(string)user["objectId"]);
		data.Add ("subscription_token",subscription_token);
		*/

		//ParseUser user = ParseUser.CurrentUser;

		//string userId = "adgfhf";//user.Get<string>("objectId");


		JSONObject data = new JSONObject ();
		data.Add ("method","register_session");
		data.Add ("user_id",ParseUser.CurrentUser.ObjectId);
		data.Add ("subscription_token",subscription_token);

		Debug.Log ("sending..");
		//client.Send (new JSONMessage (data,0,"register_session"));
		client.Send (data.ToString());
		
	}

	private void SocketConnectionClosed (object sender, EventArgs e) {
		//Debug.Log ("SocketConnectionClosed");

		socketStatus = SocketStatus.DISCONNECTED;
	}

	private void SocketError (object sender, ErrorEventArgs e) {
		Debug.Log ("socket connection error");

		socketStatus = SocketStatus.ERROR;
		
		
	}

	void OnApplicationQuit ()//close connection while application is being destroyed
	{
		if (client != null) {
			client.Close ();
		}

	}

}
