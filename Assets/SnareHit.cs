using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareHit : MonoBehaviour {

	public AudioSource audio;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			audio.Play ();
		}
	}
}
