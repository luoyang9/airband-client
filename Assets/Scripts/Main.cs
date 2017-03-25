﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using AssemblyCSharp;

public class Main : MonoBehaviour {

	SocketIOComponent socket;
	Instrument[] instruments;

	Track[] tracks;
	ArrayList players;
	bool client_ready;

	Track currentTrack;	// For challenges

	string clientID;


	// Use this for initialization



	void Start () {

		instruments = new Instrument[3];
		players = new ArrayList ();

		socket = gameObject.GetComponent<SocketIOComponent>();

		socket.On ("init", onInit);
		socket.On ("rockon:note", onRockOnNote);
		socket.On ("rockon:track", onRockOnStart);
		socket.On ("challenge:join", onJoinChallenge);
		socket.On ("challenge:ready", onChallengeReady);
		socket.On ("challenge:note", onChallengeNote);


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

		for (int x = 0; x < players.Count; x++) {
			if (((Player)players [x]).player_id.Equals (playerID)) {
				playNote (((Player)players [x]).instrument, note_id, is_pressed);
				return;
			}
		}

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

	public void leaveRockOn(){
		socket.Emit ("rockon:leave");
	}

	public void joinChallenge(){
		socket.Emit ("challenge:join");
	}

	private void onJoinChallenge(SocketIOEvent e){
		clientID = e.data.GetField ("you").ToString ();
		players = new ArrayList(JsonUtility.FromJson<ChallengePlayer[]>(e.data.GetField("players").ToString()));
		// next step
	}
	private void onNewPlayerJoin(SocketIOEvent e){
		ChallengePlayer newPlayer = JsonUtility.FromJson<ChallengePlayer> (e.data.GetField ("player").ToString ());
		players.Add (newPlayer);
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

		for (int x = 0; x < players.Count; x++) {
			ChallengePlayer curr = (ChallengePlayer)players [x];
			if (curr.id.Equals (playerID)) {
				curr.is_ready = is_ready;
				curr.track_id = track_id;
				// update
				return;
			}
		}
		Debug.LogError ("The player with id " + playerID + " could not be found");
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

		for (int x = 0; x < players.Count; x++) {
			ChallengePlayer curr = (ChallengePlayer)players [x];
			if (curr.id.Equals (player_id)) {
				curr.score += score;
				playNote (currentTrack.instrument, note_id, is_pressed);
				// rerender
				return;
			}
		}

		Debug.LogError ("challenge player with id " + player_id + " couldn't be found");

	}

	public void noteHit(int note_id, bool is_pressed){
		JSONObject data = new JSONObject ();
		data.AddField ("note_id", note_id);
		data.AddField ("is_pressed", is_pressed);

		socket.Emit ("rockon:note", data);

	}

	public void saveTrack(Track track){
		JSONObject data = new JSONObject(JsonUtility.ToJson (track));
		socket.Emit("rockon:track", data);
	}
		


	private void playNote(string instrument, int note_id, bool is_pressed){
		for (int x = 0; x < instruments.Length; x++) {
			if (((Instrument)instruments [x]).instrument_name.Equals (instrument)) {
				((Instrument)instruments [x]).playNote (note_id, is_pressed);
			}
		}
	}
			
}
