using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class PlayCymbalHit : PhysicalInstrument {

	public AudioSource centerAudio;
	public int NOTE_ID = 0;
	Main main;
	// Use this for initialization
	void Start () {
		main = getMain ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnTriggerCymbal() {
		centerAudio.Play ();
		main.noteHit (NOTE_ID, true);
	}
}
