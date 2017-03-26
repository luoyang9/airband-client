using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class Tom1Hit : PhysicalInstrument {

	public AudioSource centerAudio;
	Main main;
	public int NOTE_ID = 3;
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
			main.noteHit (NOTE_ID, true);;
		}
	}
}
