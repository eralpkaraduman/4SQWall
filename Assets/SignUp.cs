using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Parse;
using System;

public class SignUp : MonoBehaviour {

	float formWidth = 300.0f;
	float formHeight = 330.0f;
	public string email;
	public string password;
	public string password2;
	bool GUIDisabled = false;
	string message = "";
	bool networkIdle = false;
	bool dismissed = false;

	public bool showSignInButton = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (dismissed) {
			Application.LoadLevel("GetVenues");
		}
	}

	void OnGUI(){

		GUI.BeginGroup (new Rect (Screen.width/2-formWidth/2,
		                          Screen.height/2-formHeight/2,
		                          formWidth,
		                          formHeight));
		
		GUI.Box (new Rect (0,0, formWidth, formHeight),"Sign Up");

		GUI.enabled = !GUIDisabled;

		float y = 30;
		GUI.Label (new Rect(20,y,260,20),"Email:");
		email = GUI.TextField(new Rect(20, (y+=20), 260, 30), email, 25);
		GUI.Label (new Rect(20,(y+=40),260,20),"Password:");
		password = GUI.TextField(new Rect(20, (y+=20), 260, 30), password, 25);
		GUI.Label (new Rect(20,(y+=40),260,20),"Password Again:");
		password2 = GUI.TextField(new Rect(20, (y+=20), 260, 30), password2, 25);

		GUI.enabled = true;

			if(password == password2 && password.Length>1 && email.Length>1 && email.IndexOf("@")!=-1 && networkIdle==false){
				GUI.enabled = true;
			}else{
				GUI.enabled = false;
			}

			if(GUI.Button (new Rect (20, (y += 40), 260, 30), "Sign Up")){

					showSignInButton = false;
					GUIDisabled = true;
					ParseSignUp(email,password);
			}

		GUI.Label (new Rect (20, (y += 30), 260, 50),message);

		GUI.enabled = true;

		if (GUI.Button (new Rect (20, (y += 40), 260, 30), "Sign In")) {
			Application.LoadLevel("Intro");
		}

		GUI.EndGroup ();
	}

	void ParseSignUp(string _email, string _password){
		message = "Signing Up...";

		networkIdle = true;

		ParseUser user = new Parse.ParseUser ();
		user ["password"] = _password;
		user ["username"] = _email;

		try{
			user.SignUpAsync ().ContinueWith (t => {

				if (t.IsFaulted) {

					using (IEnumerator<System.Exception> enumerator = t.Exception.InnerExceptions.GetEnumerator()) {
						if (enumerator.MoveNext()) {
							ParseException error = (ParseException) enumerator.Current;
							message = error.Message+" User already exists?";
						}
					}
					showSignInButton = true;
					networkIdle = false;
					GUIDisabled = false;

				}else{
					message = "Signed Up.";
					dismissed = true;

				}

			});
		
		}catch (InvalidOperationException e){

			message = e.Message;
			showSignInButton = true;
			networkIdle = false;
			GUIDisabled = false;
		}



		
	}



}
