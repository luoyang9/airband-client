using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitKey : MonoBehaviour {

	public AudioSource audio;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerStart(Collider col) {
		if((col.transform.parent && col.transform.parent.parent && col.transform.parent.parent.GetComponent<HandModel>())) {
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
