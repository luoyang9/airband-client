using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class BongoSounder : Instrument{

	// Use this for initialization
	private AudioSource[] sounds;
	string instrument_name;

	public BongoSounder(){
		this.instrument_name = "Bongo";	
		//AudioSource[] 
		AudioSource[] highDrum = GameObject.Find ("Hi Drum").GetComponents<AudioSource>();
		Debug.Log (highDrum.Length);
		sounds = new AudioSource[4];
		sounds [0] = highDrum [0];
		sounds [1] = highDrum [1];

		AudioSource[] lowDrum = GameObject.Find ("Lo Drum").GetComponents<AudioSource> ();
		sounds [2] = lowDrum [0];
		sounds [3] = lowDrum [1];
	}
		

	public void playNote(int idx, bool is_pressed){
		Debug.Assert(idx < sounds.Length);
		sounds [idx].Play ();

	}
}
