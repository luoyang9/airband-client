using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class PickTrack :PhysicalInstrument {

	public Camera gameCamera;
	public HandController hand;
	public string track;
	private GameObject instrument;
	public GameObject bongoTrack;

	Main main;


	// Use this for initialization
	void Start () {
		this.main = getMain ();
	}

	void OnTriggerEnter(Collider col) {	
		GameObject cam = GameObject.Find ("Camera");
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle" && cam.transform.position.x > 0 ) {
			Debug.Log ("selected track " + track);

			Vector3 handMovementScale;
			Vector3 handPosition;

			if (track == "odetojoy") {
				GameObject newInstrument = Instantiate (GameObject.Find("Piano"));
				handMovementScale = new Vector3 (1.3f, 1.3f, 1.3f);
				handPosition = new Vector3 (0, 0.2f, -1.926f);
				newInstrument.transform.position = new Vector3 (0, 0, -1.9f);
				bongoTrack.SetActive (true);
				GameObject.Find ("BongoDisplay").GetComponent<NoteDisplay> ().setTrack ((Track)main.tracks[1]);
				GameObject.Find ("BongoDisplay").GetComponent<NoteDisplay> ().trackStart ();
			} else {
				GameObject newInstrument = Instantiate (GameObject.Find("Bongos"));	Debug.Log (newInstrument.gameObject.name);
				handMovementScale = new Vector3 (1, 1, 1);
				handPosition = new Vector3 (0, 0.4f, -1.926f);
				newInstrument.transform.position = new Vector3 (-2.188605f, -3.370371f, -0.1664743f);
				bongoTrack.SetActive (true);
				GameObject.Find ("BongoDisplay").GetComponent<NoteDisplay> ().setTrack ((Track)main.tracks[0]);
				GameObject.Find ("BongoDisplay").GetComponent<NoteDisplay> ().trackStart ();
			}
			GetComponent<AudioSource> ().Play ();

			gameCamera.GetComponent<LerpTransition> ().StartTransition(new Vector3 (0, 1, -2.9f), true);
			hand.transform.position = handPosition;
			hand.transform.localScale = new Vector3 (1, 1, 1);
			hand.handMovementScale = handMovementScale;

			main.joinChallenge ();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
