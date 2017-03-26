using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Net;

public class testscript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find ("SocketIO");

		var socket = go.GetComponent<SocketIOComponent> ();
		socket.Connect ();

		Debug.Log (socket.isActiveAndEnabled);
		Debug.Log (socket.IsConnected);
		socket.Emit ("test");
		//Debug.Log(socket.

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
