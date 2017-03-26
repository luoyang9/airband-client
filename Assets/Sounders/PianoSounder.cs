using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class PianoSounder : Instrument{

	AudioSource[] sounds;
	string instrument_name = "piano";

	public PianoSounder(){
		sounds = new AudioSource[7];
		sounds [0] = GameObject.Find ("A Key").GetComponent<AudioSource> ();
		sounds [1] = GameObject.Find ("B Key").GetComponent<AudioSource> ();
		sounds [2] = GameObject.Find ("C Key").GetComponent<AudioSource> ();
		sounds [3] = GameObject.Find ("D Key").GetComponent<AudioSource> ();
		sounds [4] = GameObject.Find ("E Key").GetComponent<AudioSource> ();
		sounds [5] = GameObject.Find ("F Key").GetComponent<AudioSource> ();
		sounds [6] = GameObject.Find ("G Key").GetComponent<AudioSource> ();
	}

	public void playNote(int id, bool is_pressed){
		Debug.Assert (id < sounds.Length);

		sounds [id].Play ();
	}

}
