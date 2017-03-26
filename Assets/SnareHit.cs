using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
public class SnareHit : PhysicalInstrument {

	public AudioSource audio;
	public int NOTE_ID = 2;
	Main main;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			audio.Play ();
			main.noteHit (NOTE_ID, true);
		}
	}
}
