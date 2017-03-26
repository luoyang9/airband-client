using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitKey : MonoBehaviour {

	public AudioSource centerAudio;

	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter(Collider col) {
		if(col.name == "bone3"  && col.transform.parent.gameObject.name == "middle") {
			centerAudio.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
