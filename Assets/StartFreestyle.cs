using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.SceneManagement;

public class StartFreestyle : MonoBehaviour {

	public string game;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void StartFreestyleMode() {
		SceneManager.LoadScene(game);
	}
}
