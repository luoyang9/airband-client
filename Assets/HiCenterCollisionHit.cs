using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiCenterCollisionHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.transform.parent && col.transform.parent.GetComponent<HandModel> ()) {
			var parent = transform.parent.gameObject.GetComponent<HiHitCollision> ();
			parent.OnTriggerCenterEnter ();
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.transform.parent && col.transform.parent.GetComponent<HandModel> ()) {
			var parent = transform.parent.gameObject.GetComponent<HiHitCollision> ();
			parent.OnTriggerCenterExit ();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
