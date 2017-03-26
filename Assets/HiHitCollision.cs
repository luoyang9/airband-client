using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class HiHitCollision : PhysicalInstrument {

	//public AudioSource audio;
	//public AudioSource edgeAudio;

	public int CENTER_NOTE_ID = 2;
	public int EDGE_NOTE_ID = 3;
	Main main;

	private bool centerTriggered;

	void Awake() {
		centerTriggered = false;
	}
	void Start(){
		main = getMain ();
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
				main.noteHit (CENTER_NOTE_ID, true);
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
