using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToGame : MonoBehaviour {

	private string mode;

	public GameObject instrument;
	public Camera gameCamera;
	public HandController hand;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			StartGame ();
		}
	}

	void StartGame() {

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

		gameCamera.GetComponent<LerpTransition> ().StartTransition();
		hand.transform.position = handPosition;
		hand.transform.localScale = new Vector3 (1, 1, 1);
		hand.handMovementScale = handMovementScale;
		if (mode == "challenge") {
			//challenge stuff
		} else {
			Main main = GameObject.Find ("Main").GetComponent<Main> ();
			main.onRockOnJoin (instrument.name.ToLower ());
		}
	}

	public void SetGameMode(string mode) {
		this.mode = mode;
	}
}
