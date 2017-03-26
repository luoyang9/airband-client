using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using AssemblyCSharp;

public class Main : MonoBehaviour {

	SocketIOComponent socket;
	Dictionary<string, Instrument> instruments;

	Track[] tracks;
	Dictionary<string, Player> players;
	Dictionary<string, ChallengePlayer> chalPlayers;
	bool client_ready;

	Track currentTrack;	// For challenges

	string clientID;

	bool challengeMode = false;


	// Use this for initialization



	void Start () {
		GameObject go = GameObject.Find ("Main");
		socket = go.GetComponent<SocketIOComponent>();


		instruments = new Dictionary<string, Instrument> ();
		instruments.Add("bongo", gameObject.GetComponent<BongoSounder>();

		socket.On ("init", onInit);
		socket.On ("rockon:join", onRockOnJoin);
		socket.On ("rockon:player_join", onRockOnPlayerJoin);
		socket.On ("rockon:note", onRockOnNote);
		socket.On ("rockon:track", onRockOnStart);
		socket.On ("challenge:join", onJoinChallenge);
		socket.On ("challenge:player_join", onChallengePlayerJoin);
		socket.On ("challenge:ready", onChallengeReady);
		socket.On ("challenge:note", onChallengeNote);
		socket.Emit ("test");
		Debug.Log ("Sockets opened");
	}

	private void onInit(SocketIOEvent e){
		tracks = JsonUtility.FromJson<Track[]> (e.data.ToString());
		// Display tracks in carosuel

	}

	private void onRockOnNote(SocketIOEvent e){
		string playerID = "";
		e.data.GetField(ref playerID, "player_id");
		int note_id = 0;
		e.data.GetField (ref note_id, "note_id");
		bool is_pressed = true;
		e.data.GetField (ref is_pressed, "is_pressed");


		//playNote (players [playerID].instrument, note_id, is_pressed);
		Debug.Log ("back");

	}

	private void onRockOnStart(SocketIOEvent e){
		currentTrack = JsonUtility.FromJson<Track> (e.data.GetField ("track").ToString());
		// do stuff to start rock on
	}

	public void joinRockOn(string instrument){
		JSONObject data = new JSONObject ();
		data.AddField ("instrument", instrument);
		socket.Emit("rockon:join", data);
		// Change ui
	}
	
	public void onRockOnJoin(SocketIOEvent e){
		players = new Dictionary<string, Player> ();
		int count = e.data.GetField ("players").Count;
		for (int x = 0; x < count; x++) {
			Player curr = JsonUtility.FromJson<Player>(e.data.GetField ("players").GetField (x.ToString()).ToString());
			players.Add (curr.player_id, curr);
		}
		// refresh display
	}

	public void onRockOnPlayerJoin(SocketIOEvent e){
		Debug.Assert (players != null);
		Player curr = JsonUtility.FromJson<Player> (e.data.GetField ("player").ToString ());

		players.Add (curr.player_id, curr);

		//refresh display
	}

	public void leaveRockOn(){
		socket.Emit ("rockon:leave");
	}

	public void joinChallenge(){
		socket.Emit ("challenge:join");
	}

	private void onJoinChallenge(SocketIOEvent e){
		clientID = e.data.GetField ("you").ToString ();
		ChallengePlayer[] joinedPlayers = JsonUtility.FromJson<ChallengePlayer[]>(e.data.GetField("players").ToString());

		chalPlayers = new Dictionary<string, ChallengePlayer> ();
		foreach (ChallengePlayer chal in joinedPlayers) {
			chalPlayers.Add (chal.id, chal);
		}
		// update;
	}
	private void onChallengePlayerJoin(SocketIOEvent e){
		ChallengePlayer newPlayer = JsonUtility.FromJson<ChallengePlayer> (e.data.GetField ("player").ToString ());
		chalPlayers.Add (newPlayer.id, newPlayer);
		// refresh display;
	}

	public void challengeReady(bool ready, int track_id){
		client_ready = ready;
		JSONObject data = new JSONObject ();
		data.AddField ("is_ready", ready);
		data.AddField ("track_id", track_id);

		socket.Emit ("challenge:ready", data);
	}

	private void onChallengeReady(SocketIOEvent e){
		string playerID = "";
		bool is_ready = false;
		int track_id = 0;

		e.data.GetField (ref playerID, "player_id");
		e.data.GetField (ref is_ready, "is_ready");
		e.data.GetField (ref track_id, "track_id");

		ChallengePlayer currPlayer = chalPlayers [playerID];
		currPlayer.is_ready = is_ready;
		currPlayer.track_id = track_id;
	}

	private void onChallengeNote(SocketIOEvent e){
		string player_id = "";
		int score = 0;
		int note_id = 0;
		bool is_pressed = true;

		e.data.GetField (ref player_id, "player_id");
		e.data.GetField (ref score, "score");
		e.data.GetField (ref note_id, "note_id");
		e.data.GetField (ref is_pressed, "is_pressed");

		chalPlayers [player_id].score += score;
		playNote (currentTrack.instrument, note_id, is_pressed);

	}
	/*
	public void noteHitChallenge(int note_id, bool is_pressed){
		JSONObject data = new JSONObject ();
		data.AddField ("note_id", note_id);
		data.AddField ("is_pressed", is_pressed);

		socket.Emit ("challenge:note", data);

	}*/

	public void noteHit(int note_id, bool is_pressed){
		JSONObject data = new JSONObject ();
		data.AddField ("note_id", note_id);
		data.AddField ("is_pressed", is_pressed);

		if (challengeMode)
			socket.Emit ("challenge:note", data);
		else
			socket.Emit ("rockon:note", data);
		Debug.Log ("Note hit");
	}

	public void saveTrack(Track track){
		JSONObject data = new JSONObject(JsonUtility.ToJson (track));
		socket.Emit("rockon:track", data);
	}
		


	private void playNote(string instrument, int note_id, bool is_pressed){
		instruments [instrument].playNote (note_id, is_pressed);
	}
			
}
