using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;

public class Intro : MonoBehaviour {

	float formWidth = 300.0f;
	float formHeight = 300.0f;
	bool dismissed = false;
	public string email;
	public string password;
	string message = "";
	bool networkIdle = true; 

	// Use this for initialization
	void Start () {
	
		if (ParseUser.CurrentUser != null)
		{
			email = ParseUser.CurrentUser.Username;

			Application.LoadLevel("GetVenues");
		}
	}

	// Update is called once per frame
	void Update () {

		if (dismissed) {
			Application.LoadLevel("GetVenues");
		}

	}

	void OnGUI(){

		GUI.enabled = networkIdle;
		
		GUI.BeginGroup (new Rect (Screen.width/2-formWidth/2,
		                          Screen.height/2-formHeight/2,
		                          formWidth,
		                          formHeight));
		
		GUI.Box (new Rect (0,0, formWidth, formHeight),"Sign In");
		
		float y = 30;

		GUI.Label (new Rect(20,y,260,20),"Email:");
		email = GUI.TextField(new Rect(20, (y+=20), 260, 30), email, 25);
		GUI.Label (new Rect(20,(y+=40),260,20),"Password:");
		password = GUI.TextField(new Rect(20, (y+=20), 260, 30), password, 25);
		if (GUI.Button (new Rect (20, (y += 50), 260, 30), "Sign In")) {
			message = "Signing In...";
			ParseSignIn(email,password);
		};

		GUI.Label (new Rect (20, (y += 40), 260, 30),message);

		if(GUI.Button(new Rect(20, (y+=50), 260, 30), "Sign Up")) {
			Application.LoadLevel("SignUp");
		};

		GUI.EndGroup ();
	
	}

	void ParseSignIn(string _username, string _password){
	
		networkIdle = false;

		try{
			ParseUser.LogInAsync(_username, _password).ContinueWith(t =>
			                                                      {
				if (t.IsFaulted || t.IsCanceled)
				{
					using (IEnumerator<System.Exception> enumerator = t.Exception.InnerExceptions.GetEnumerator()) {
						if (enumerator.MoveNext()) {
							ParseException error = (ParseException) enumerator.Current;
							message = error.Message;
							networkIdle = true;
						}
					}
				}
				else
				{
					message = "Signed In.";
					dismissed = true;
				}


			});
		}catch (InvalidOperationException e){
			message = e.Message;
			networkIdle = true;
		}
	}
}
