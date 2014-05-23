using UnityEngine;
using System.Collections;
using SocketIOClient;
using System;

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
		if (client.IsConnected) {
			//Debug.Log("con");
		}

	}

	private void SocketMessage (object sender, MessageEventArgs e) {
		Debug.Log ("socket message received");

		if (e != null) {
			string eventName = e.Message.Event;
			Debug.Log("event : "+eventName);
		}





		/*
		if ( e!= null && e.Message.Event == "message") {




			string msg = e.Message.MessageText;
			Debug.Log("msg "+msg);
			//process(msg);
		}else if(e==null){
			Debug.Log("e is null");
		}else{

		}
		*/
	}

	private void SocketOpened(object sender, EventArgs e) {

		Debug.Log ("socket opened");
		//invoke when socket opened
	}

	private void SocketConnectionClosed (object sender, EventArgs e) {
		Debug.Log ("SocketConnectionClosed");
	}




	private void SocketError (object sender, ErrorEventArgs e) {
		Debug.Log ("socket error "+e.Message);


	}

}
