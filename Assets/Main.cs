using UnityEngine;
using System.Collections;
using System;

public class Main : MonoBehaviour {

	public static Main Instance { get; private set; }


	//public String FOURSQUARE_CLIENT_ID = "ESAE4V4MAD1KXS2AGILLGAHVHQBAS3DZL05H32XVZV2YBV0S";
	//public String FOURSQUARE_REDIRECT_URI = "https://squarewall.parseapp.com/foursquare_redirect";
	/*
	enum FoursquareConnectState{ 
		NOT_CONNECTED,
		CONNECTING,
		CONNECTED,
		FAILED};
	*/
	//FoursquareConnectState foursquareConnectState = FoursquareConnectState.NOT_CONNECTED;

	void Awake()
	{
		// Save a reference to the AudioHandler component as our singleton instance
		Instance = this;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SignUp>().enabled = false;
	}

	/*
	void Connect4SQ(){

		Debug.Log ("au "+Application.absoluteURL);

		String connectURL = "https://foursquare.com/oauth2/authenticate"+
							"?client_id="+FOURSQUARE_CLIENT_ID+
							"&response_type=code"+
							"&redirect_uri="+FOURSQUARE_REDIRECT_URI;
		
		Application.OpenURL (connectURL);
	}
	*/

	public void ParseSignUpComplete(){
		Debug.Log ("yolo");
		//GetComponent<SignUp>().enabled = false;
	}

}
