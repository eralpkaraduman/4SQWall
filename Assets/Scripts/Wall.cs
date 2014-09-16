using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Boomlagoon.JSON;
using Parse;

public class Wall : MonoBehaviour {

	public SocketConnection socketConnection;
	public CheckinSpawner spawner;
	public GameObject currentCheckinAlert;
	private WallState state;

	private ParseObject lastCheckinReceived = null;

	public Transform alertPivot;

	private float reloadTimer = 0;
	public float reloadInterval = 5*60;

	private static int MAX_CHECKINS = 10;

	enum WallState{
		GETTING_VENUE,
		GETTING_CHECK_INS,
		CHECKINS_RETRIEVED,
		CHECKINS_PROCESSED,
		SOCKET_CHECKIN_RECEIVED,
		SOCKET_CHECKIN_PROCESSED
	}

	long timeStamp = 0;
	ArrayList checkinList;


	// Use this for initialization
	void Start () {

		spawner.OnAlertQueueCompleted += spawnerAlertQueueCompleted;

		socketConnection.checkInReceived += onSocketCheckinReceived;
		socketConnection.Begin ();
		ReloadCheckins ();
		//spawner.reloadWheelWithParseObjects (null);
	}
	
	// Update is called once per frame
	void Update () {
		if (state == WallState.CHECKINS_RETRIEVED) {
			spawner.reloadWheelWithParseObjects(checkinList);
			state = WallState.CHECKINS_PROCESSED;
		}

		if (state == WallState.SOCKET_CHECKIN_RECEIVED) {

			state = WallState.SOCKET_CHECKIN_PROCESSED;
			spawner.alertNewCheckIn (lastCheckinReceived,alertPivot);

		}


		reloadTimer += Time.deltaTime;
		if (reloadTimer > reloadInterval) {

			ReloadCheckins();

			reloadTimer = 0;
			
		}
	}

	void OnGUI(){
		if (state != WallState.CHECKINS_PROCESSED) {

			Color oldColor = GUI.color;
			Color color = Color.white;
			color.a = 0.5f;
			GUI.color = color;
			GUI.Label (new Rect (20, 20, 400, 30), state.ToString ());
			GUI.color = oldColor;
		}
	}

	void ReloadCheckins(){

		state = WallState.GETTING_VENUE;

		Debug.Log("getting subscribed venue..");

		ParseUser user = ParseUser.CurrentUser;

		var relation = user.GetRelation<ParseObject> ("subscribedVenue");

		relation.Query.FirstAsync().ContinueWith(t =>{

			ParseObject venue = t.Result;
			GetCheckinsOfVenue(venue);

		});

	}

	void GetCheckinsOfVenue(ParseObject venue){

		state = WallState.GETTING_CHECK_INS;

		Debug.Log("getting check ins..");
		
		if (timeStamp == 0) {
			
			DateTime dateToUse = DateTime.Now;

			//dateToUse.Year
			DateTime timeToUse = new DateTime(dateToUse.Year, 1, 1, 0, 0, 0); //10:15:30 AM
			DateTime dateWithRightTime = dateToUse.Date.Add(timeToUse.TimeOfDay);
			timeStamp = UnixTimestampFromDateTime(dateWithRightTime);
			 /*test old*/ timeStamp -= 800000;
			
		}
		
		Debug.Log("getting checkins since "+timeStamp+" ...");


		var relation = venue.GetRelation<ParseObject> ("checkins");
		var query = relation.Query;

		query.WhereGreaterThanOrEqualTo("cretedAtFoursquare",timeStamp).FindAsync().ContinueWith(t =>{

			IEnumerable<ParseObject> checkins = (IEnumerable<ParseObject>)t.Result;

			checkinList = new ArrayList();

			int numCheckins = checkinList.Count;
			if(numCheckins>MAX_CHECKINS)numCheckins = MAX_CHECKINS;

			int index = 0;
			foreach(ParseObject checkin in checkins){
				checkinList.Add(checkin);
				index ++;
				if(index+1>MAX_CHECKINS)break;
			}

			Debug.Log("Retrieved "+checkinList.Count+" check ins");

			state = WallState.CHECKINS_RETRIEVED;

		});

	}

	public void onSocketCheckinReceived(JSONObject jsonCheckin){

		ParseObject checkin = new ParseObject("Checkin");

		checkin ["objectId"] = jsonCheckin.GetString ("objectId");
		checkin ["foursquareId"] = jsonCheckin.GetString ("foursquareId");
		checkin ["userFirstName"] = jsonCheckin.GetString ("userFirstName");
		checkin ["cretedAtFoursquare"] = jsonCheckin.GetNumber ("cretedAtFoursquare");
		checkin ["userGender"] = jsonCheckin.GetString ("userGender");
		checkin ["userPhotoPrefix"] = jsonCheckin.GetString ("userPhotoPrefix");
		checkin ["userPhotoSuffix"] = jsonCheckin.GetString ("userPhotoSuffix");
		checkin ["userlastName"] = jsonCheckin.GetString ("userlastName");
		checkin ["foursquareTimeZoneOffset"] = jsonCheckin.GetNumber ("foursquareTimeZoneOffset");
		checkin ["venueFoursquareId"] = jsonCheckin.GetString ("venueFoursquareId");

		lastCheckinReceived = checkin;
		state = WallState.SOCKET_CHECKIN_RECEIVED;

	}

	public static long UnixTimestampFromDateTime(DateTime date)
	{
		long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
		unixTimestamp /= TimeSpan.TicksPerSecond;
		return unixTimestamp;
	}

	public static DateTime DateTimeFromUnixTimeStamp(long unixTimestamp)
	{
		DateTime unixYear0 = new DateTime(1970, 1, 1);
		long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
		DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
		return dtUnix;
	}


	public void spawnerAlertQueueCompleted(){

		ReloadCheckins ();
	
	}



}
