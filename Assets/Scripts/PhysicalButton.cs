using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalButton : MonoBehaviour {

	public string action;

	void OnTriggerEnter(Collider col){
		Debug.Log ("collision");
		if (col.transform.parent.gameObject.name == "middle" && col.name == "bone3") {

			switch (action) {
			case "challenge":
				Debug.Log ("challenge");
				break;
			case "rockon":
				Debug.Log ("Rock on");
				break;
			case "record":
				Debug.Log ("record");
				break;
			}


			Debug.Log ("touched");
		}
	}
		
}
