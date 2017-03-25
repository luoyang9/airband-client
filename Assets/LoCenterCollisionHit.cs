using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoCenterCollisionHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.transform.parent && col.transform.parent.GetComponent<HandModel> ()) {
			var parent = transform.parent.gameObject.GetComponent<HitCollision> ();
			parent.OnTriggerCenterEnter ();
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.transform.parent && col.transform.parent.GetComponent<HandModel> ()) {
			var parent = transform.parent.gameObject.GetComponent<HitCollision> ();
			parent.OnTriggerCenterExit ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
