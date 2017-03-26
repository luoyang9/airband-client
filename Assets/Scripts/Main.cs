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
		instruments.Add("bongos", new BongoSounder());
		instruments.Add ("piano", new PianoSounder ());
		instruments.Add ("drums", new DrumSounder ());

		socket.On ("init", onInit);
		socket.On ("rockon:join", onRockOnJoin);
		socket.On ("rockon:player_join", onRockOnPlayerJoin);
		socket.On ("rockon:note", onRockOnNote);
		//socket.On ("rockon:track", onRockOnStart);
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
		Debug.Log ("onRockOnNote");
		string playerID = "";

		e.data.GetField(ref playerID, "player_id");
		int note_id = 0;
		e.data.GetField (ref note_id, "note_id");
		bool is_pressed = true;
		e.data.GetField (ref is_pressed, "is_pressed");

		if (playerID == clientID) {
			Debug.Log ("avoid echo");
			return;
		}

		//Debug.Log (players [playerID].instrument);
		playNote (players [playerID].instrument, note_id, is_pressed);


		//instruments ["bongo"].playNote (note_id, is_pressed);

	}
	/*
	private void onRockOnStart(SocketIOEvent e){
		currentTrack = JsonUtility.FromJson<Track> (e.data.GetField ("track").ToString());
		// do stuff to start rock on
	}*/

	public void joinRockOn(string instrument){
		Debug.Log ("JoinRockOn");
		JSONObject data = new JSONObject ();
		data.AddField ("instrument", instrument);
		socket.Emit("rockon:join", data);
		// Change ui
	}

	public void onRockOnJoin(SocketIOEvent e){
		Debug.Log ("onRockOnJoin");
		clientID = e.data.GetField ("you").ToString ();
		Debug.Log ("Set id as " + clientID);
		players = new Dictionary<string, Player> ();

		int count = e.data.GetField ("players").Count;
		Debug.Log (count);
		Debug.Log (e.data);

		for (int x = 0; x < count; x++) {
			Player curr = JsonUtility.FromJson<Player>(e.data.GetField ("players")[x].ToString());
			Debug.Log (curr.ToString ());
			players.Add (curr.id, curr);
		}

		// refresh display
	}

	public void onRockOnPlayerJoin(SocketIOEvent e){
		Debug.Log ("on rock on player join");
		Debug.Assert (players != null);
		Player curr = JsonUtility.FromJson<Player> (e.data.GetField ("player").ToString ());
		//Debug.Log (e.data.GetField("player").ToString());
		Debug.Assert (curr != null);
		players.Add (curr.id, curr);
		Debug.Log (curr.ToString());


		//refresh display
	}

	public void leaveRockOn(){
		socket.Emit ("rockon:leave");
	}

	public void joinChallenge(){
		socket.Emit ("challenge:join");
	}

	private void onJoinChallenge(SocketIOEvent e){
		Debug.Log ("onJoinChallenge");
		clientID = e.data.GetField ("you").ToString ();
		ChallengePlayer[] joinedPlayers = JsonUtility.FromJson<ChallengePlayer[]>(e.data.GetField("players").ToString());

		chalPlayers = new Dictionary<string, ChallengePlayer> ();
		foreach (ChallengePlayer chal in joinedPlayers) {
			chalPlayers.Add (chal.id, chal);
		}
		// update;
	}
	private void onChallengePlayerJoin(SocketIOEvent e){
		Debug.Log ("onChallengePlayerJoin");
		ChallengePlayer newPlayer = JsonUtility.FromJson<ChallengePlayer> (e.data.GetField ("player").ToString ());
		chalPlayers.Add (newPlayer.id, newPlayer);
		// refresh display;
	}

	public void challengeReady(bool ready, int track_id){
		Debug.Log ("ChallengeReady");
		client_ready = ready;
		JSONObject data = new JSONObject ();
		data.AddField ("is_ready", ready);
		data.AddField ("track_id", track_id);

		socket.Emit ("challenge:ready", data);
	}

	private void onChallengeReady(SocketIOEvent e){
		Debug.Log ("onChallengeReady");
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
		Debug.Log ("onChallengeNote");
		string player_id = "";
		int score = 0;
		int note_id = 0;
		bool is_pressed = true;

		e.data.GetField (ref player_id, "player_id");
		e.data.GetField (ref score, "score");
		e.data.GetField (ref note_id, "note_id");
		e.data.GetField (ref is_pressed, "is_pressed");

		if (player_id == clientID) {
			return;
		}

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
		Debug.Log ("Note Hit");
		JSONObject data = new JSONObject ();
		data.AddField ("note_id", note_id);
		data.AddField ("is_pressed", is_pressed);

		if (challengeMode)
			socket.Emit ("challenge:note", data);
		else
			socket.Emit ("rockon:note", data);
		//Debug.Log ("Note hit");
	}

	public void saveTrack(Track track){
		Debug.Log ("saveTrack");
		JSONObject data = new JSONObject(JsonUtility.ToJson (track));
		socket.Emit("rockon:track", data);
	}
		


	private void playNote(string instrument, int note_id, bool is_pressed){
		Debug.Log ("playNote");
		instruments [instrument].playNote (note_id, is_pressed);
	}
			
}
