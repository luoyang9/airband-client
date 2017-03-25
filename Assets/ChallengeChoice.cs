using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeChoice : MonoBehaviour {

	public static string choice;
	public string btnChoice;

	// Use this for initialization
	void Start () {
		
	}

	public void SelectChoice() {
		choice = btnChoice;
		SceneManager.LoadScene ("Challenge");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
