using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickInstrumentClick : MonoBehaviour {

	public GameObject pianoBtn;
	public GameObject drumBtn;
	public GameObject bongoBtn;
	public GameObject freeStyleBtn;
	public string mode;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "bone3" && col.transform.parent.gameObject.name == "middle") {
			if (mode == "challenge") {
				
				GameObject.Find("Camera").GetComponent<LerpTransition> ().StartTransition (new Vector3 (1.22f, 3.27f, 5.77f), false);
				GameObject.Find("HandController").transform.position = new Vector3 (1.42f, 2.52f, 7.31f);
			} else {
				pianoBtn.gameObject.SetActive (true);
				bongoBtn.gameObject.SetActive (true);
				drumBtn.gameObject.SetActive (true);
				pianoBtn.gameObject.GetComponent<GoToGame> ().SetGameMode (mode);
				bongoBtn.gameObject.GetComponent<GoToGame> ().SetGameMode (mode);
				drumBtn.gameObject.GetComponent<GoToGame> ().SetGameMode (mode);

				gameObject.SetActive (false);
				freeStyleBtn.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
