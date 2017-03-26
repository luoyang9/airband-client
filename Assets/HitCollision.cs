using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class HitCollision : PhysicalInstrument  {

	public AudioSource centerAudio;
	public AudioSource edgeAudio;

	private bool centerTriggered;
	public bool challenge = false;
	public int CENTER_NOTE_ID = 0;
	public int EDGE_NOTE_ID = 1;

	Main main;

	void Start() {
		centerTriggered = false;
		this.main = getMain ();

		Debug.Assert (main != null);
	}

	private bool isHand(Collider col) {
		if (col.transform.parent && col.transform.parent.GetComponent<HandModel> () ) {
			return true;
		} 
		return false;
	}

	void OnTriggerEnter(Collider col) {
		if(isHand(col)) {
			if (centerTriggered) {
				centerAudio.Play ();
				main.noteHit(CENTER_NOTE_ID, true);
			} else {
				main.noteHit (EDGE_NOTE_ID, true);
			}
		}
	}

	public void OnTriggerCenterEnter() {
		centerTriggered = true;
	}

	public void OnTriggerCenterExit() {
		centerTriggered = false;
	}

}
