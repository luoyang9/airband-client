using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToGame : MonoBehaviour {
	
	public static string choice;

	private string mode;

	public string instrument;

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
		if (mode == "challenge") {
			choice = instrument;
			SceneManager.LoadScene ("Challenge");
		} else if (mode == "freestyle") {
			switch (instrument) {
			case "piano":
				SceneManager.LoadScene ("piano");
				break;
			case "drums":
				SceneManager.LoadScene ("drums-backup");
				break;
			case "bongos":
				SceneManager.LoadScene ("bongos-backup");
				break;
			}
		}
	}

	public void SetGameMode(string mode) {
		this.mode = mode;
	}
}
