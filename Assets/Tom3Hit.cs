using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class Tom3Hit : PhysicalInstrument {

	public AudioSource centerAudio;
	public int NOTE_ID = 1;
	Main main;

	// Use this for initialization
	void Start () {
		main = this.getMain ();
	}

	// Update is called once per frame
	void Update () {

	}


	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			centerAudio.Play ();
			main.noteHit (NOTE_ID, true);
		}
	}
}
