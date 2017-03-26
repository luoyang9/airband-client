using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class HitKey : PhysicalInstrument {

	public AudioSource centerAudio;
	public int NOTE_ID;
	Main main;
	// Use this for initialization
	void Start () {
		main = this.getMain ();
	}

	void OnTriggerEnter(Collider col) {
		if(col.name == "bone3"  && col.transform.parent.gameObject.name == "middle") {
			centerAudio.Play ();
			main.noteHit (NOTE_ID, true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
