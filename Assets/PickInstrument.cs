using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickInstrument : MonoBehaviour { 
	
	public GameObject drums;
	public GameObject bongos;
	public GameObject piano;

	// Use this for initialization
	void Start () {
		switch (GoToGame.choice) {
		case "piano":
			Instantiate (piano);
					break;
		case "bongos":
			Instantiate (bongos);
					break;
		case "drums":
			Instantiate (drums);
					break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
