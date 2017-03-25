using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using AssemblyCSharp;


public class Main : MonoBehaviour {

	SocketIOComponent socket;
	Track[] tracks;


	// Use this for initialization



	void Start () {
		socket = gameObject.GetComponent<SocketIOComponent>();

		socket.On("init", onInit);
		socket.On ("rockon:note", onRockOnNote);


	}

	private void onInit(SocketIOEvent e){
		tracks = JsonUtility.FromJson<Track[]> (e.data);
		// Display tracks in carosuel

	}

	private void onRockOnNote(SocketIOEvent e){
		JSONObject data = new JSONObject (e.data);
		string playerID = data.GetField ("player_id");
		int note_id = data.GetField ("note_id");
		bool is_pressed = data.GetField ("is_pressed");

	}

	private void joinRockOn(string instrument){
		JSONObject data = new JSONObject ();
		data.AddField ("instrument", instrument);
		socket.Emit("rockon:join", data.ToString());
		// Change ui
	}

	private void playNote(int note_id, bool is_pressed){
		JSONObject data = new JSONObject ();
		data.AddField ("note_id", note_id);
		data.AddField ("is_pressed", is_pressed);

		socket.Emit ("rockon:note", data.ToString ());
	}

	private void 
		


	
}
