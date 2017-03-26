using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class HitKey : PhysicalInstrument {

	public AudioSource centerAudio;
	public int NOTE_ID;
	private bool isPressed;
	Main main;
	// Use this for initialization
	void Start () {
		main = this.getMain ();
	}

	void OnTriggerEnter(Collider col) {
		if(col.name == "bone3"  && col.transform.parent.gameObject.name == "middle" && !isPressed) {
			centerAudio.Play ();
			main.noteHit (NOTE_ID, true);
			StartCoroutine (PressDown ());
		}
	}

	IEnumerator PressDown() {
		isPressed = true;
		transform.localEulerAngles = new Vector3 (-2, 0, 0);
		yield return new WaitForSeconds(0.2f);
		transform.localEulerAngles = new Vector3 (0, 0, 0);
		isPressed = false;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
