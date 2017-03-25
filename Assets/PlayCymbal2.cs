using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCymbal2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			Debug.Log ("middle hit!");
			var parent = transform.parent.gameObject.GetComponent<PlayCymbalHit> ();
			parent.OnTriggerCymbal ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
