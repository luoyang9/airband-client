using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class BongoSounder : Instrument {

	// Use this for initialization
	public AudioSource[] sounds;
	string instrument_name;

	void Awake(){
		this.instrument_name = "Bongo";	
	}


	public void playNote(int idx, bool is_pressed){
		Debug.Assert(idx < sounds.Length);
		sounds [idx].Play ();

	}
}
