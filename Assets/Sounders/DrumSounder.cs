using System;
using UnityEngine;
using AssemblyCSharp;


namespace AssemblyCSharp
{
	public class DrumSounder: Instrument
	{
		AudioSource[] sounds;
		String instrument_type = "drum";

		public DrumSounder ()
		{
			sounds = new AudioSource[5];

			sounds [0] = GameObject.Find ("cymbal").GetComponent<AudioSource> ();
			sounds [1] = GameObject.Find ("ring_floor").GetComponent<AudioSource> ();
			sounds [2] = GameObject.Find ("ring_snare").GetComponent<AudioSource> ();
			sounds [3] = GameObject.Find ("ring_tom_kanan").GetComponent<AudioSource> ();
			sounds [4] = GameObject.Find ("ring_tom_kiri").GetComponent<AudioSource> ();

		}

		public void playNote(int id, bool is_pressed){
			Debug.Assert (id < sounds.Length);

			sounds [id].Play ();
		}
	}
}

