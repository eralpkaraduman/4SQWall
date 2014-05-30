using UnityEngine;
using System.Collections;
using SocketIOClient;
using System;
using SimpleJson;
using System.Reflection;
using System.Collections.Generic;

public class SocketTest : MonoBehaviour {

	Client client = null;

	// Use this for initialization
	void Start () {

		client = new Client ("http://127.0.0.1:3000?subscription_token=1234");

		//Client client = new Client ("http://localhost:3000");



		client.Opened += SocketOpened;
		client.Message += SocketMessage;
		client.SocketConnectionClosed += SocketConnectionClosed;
		client.Error +=SocketError;


		Debug.Log ("connecting..");
		client.Connect ();


	}
	
	// Update is called once per frame
	void Update () {

		bool connected = false;
		if (client!=null) {
			if (client.IsConnected) {
				connected = true;
			}
		}

		Debug.Log("connnected: "+connected);
	}

	private void SocketMessage (object sender, MessageEventArgs e) {

		Debug.Log ("socket message received");

		if ( e!= null && e.Message.Event == "message") {

			string subscribed = (string)e.Message.Json.args[0];
			if(subscribed == "SUBSCRIPTION:SUCCESS"){
				Debug.Log("Subscribed");
			}else if(subscribed == "SUBSCRIPTION:FAILED"){
				Debug.Log("Subscription Failed");
			}

		}

	}

	private void SocketOpened(object sender, EventArgs e) {
		
		Debug.Log ("socket opened");

		string uid = "4564756";
		string t = "1234";

		object data = new {
			method= "subscribe",
			user_id = uid,
			subscription_token = t
		};

		string jsonData = SimpleJson.SimpleJson.SerializeObject (data);

		client.Send (jsonData);


	}

	private void SocketConnectionClosed (object sender, EventArgs e) {
		Debug.Log ("SocketConnectionClosed");
	}

	private void SocketError (object sender, ErrorEventArgs e) {
		Debug.Log ("socket error "+e.Message);


	}

	void OnApplicationQuit ()//close connection while application is being destroyed
	{
		client.Close ();
	}
	
}
