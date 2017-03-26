using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiHitCollision : MonoBehaviour {

	public AudioSource centerAudio;
	public AudioSource edgeAudio;

	private bool centerTriggered;

	void Start() {
		centerTriggered = false;
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
			} else {
				edgeAudio.Play ();
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
