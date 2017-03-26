using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickTrack : MonoBehaviour {

	public Camera gameCamera;
	public HandController hand;
	public string track;
	private GameObject instrument;


	// Use this for initialization
	void Start () {
	}

	public void SetInstrument(GameObject instrument) {
		this.instrument = instrument;
	}

	void OnTriggerEnter(Collider col) {	
		GameObject cam = GameObject.Find ("Camera");
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle" && cam.transform.position.x > 0 ) {
			Debug.Log ("selected track " + track);

			Vector3 handMovementScale;
			Vector3 handPosition;

			GameObject newInstrument = Instantiate (instrument);

			if (newInstrument.gameObject.name == "Drums(Clone)") {
				handMovementScale = new Vector3 (4, 4, 4);
				handPosition = new Vector3 (0, -0.2f, -1.926f); 
				newInstrument.transform.position = new Vector3 (0, -0.8137832f, -1.23f);
			} else if(newInstrument.gameObject.name == "Piano(Clone)"){
				handMovementScale = new Vector3 (1.3f, 1.3f, 1.3f);
				handPosition = new Vector3 (0, 0.2f, -1.926f);
				newInstrument.transform.position = new Vector3 (0, 0, -1.9f);
			} else {
				Debug.Log (newInstrument.gameObject.name);
				handMovementScale = new Vector3 (1, 1, 1);
				handPosition = new Vector3 (0, 0.4f, -1.926f);
				newInstrument.transform.position = new Vector3 (-2.188605f, -3.370371f, -0.1664743f);
			}

			gameCamera.GetComponent<LerpTransition> ().StartTransition(new Vector3 (0, 1, -2.9f), true);
			hand.transform.position = handPosition;
			hand.transform.localScale = new Vector3 (1, 1, 1);
			hand.handMovementScale = handMovementScale;

			Main main = GameObject.Find ("Main").GetComponent<Main> ();
			main.joinRockOn (instrument.name.ToLower ());
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
