using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCymbalHit : MonoBehaviour {

	public AudioSource centerAudio;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void OnTriggerCymbal() {
		centerAudio.Play ();
	}
}
