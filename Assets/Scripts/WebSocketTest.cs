using UnityEngine;
using System.Collections;
using WebSocket4Net;
using System;

public class WebSocketTest : MonoBehaviour {

	WebSocket websocket = null;

	// Use this for initialization
	void Start () {

		websocket = new WebSocket ("ws://localhost:3000/");
		websocket.Opened += new EventHandler(websocket_Opened);
		//websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
		//websocket.Closed += new EventHandler(websocket_Closed);
		//websocket.MessageReceived += new EventHandler(websocket_MessageReceived);


		Debug.Log ("opening...");
		websocket.Open();
	}

	private void websocket_Opened(object sender, EventArgs e)
	{
		Debug.Log ("open");

		websocket.Send("Hello World!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
