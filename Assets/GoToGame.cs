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
		instrument.SetActive (true);

		Vector3 handMovementScale;
		Vector3 handPosition;
		if (instrument.gameObject.name == "Drums") {
			handMovementScale = new Vector3 (4, 4, 4);
			handPosition = new Vector3 (0, -0.2f, -1.926f); 
		} else if(instrument.gameObject.name == "Piano"){
			handMovementScale = new Vector3 (1.3f, 1.3f, 1.3f);
			handPosition = new Vector3 (0, 0.2f, -1.926f);
		} else {
			handMovementScale = new Vector3 (1, 1, 1);
			handPosition = new Vector3 (0, 0.4f, -1.926f);
		}

		gameCamera.GetComponent<LerpTransition> ().StartTransition();
		hand.transform.position = handPosition;
		hand.transform.localScale = new Vector3 (1, 1, 1);
		hand.handMovementScale = handMovementScale;
		if (mode == "challenge") {
			//challenge stuff
		} 
	}

	public void SetGameMode(string mode) {
		this.mode = mode;
	}
}
