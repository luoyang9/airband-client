using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

	int score;
	public TextMesh textField;
	// Use this for initialization
	void Start () {
		score = 0;
		textField = transform.FindChild ("Score").GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		textField.text = score.ToString ();
	}

	public void updateScore(int score){
		this.score += score;
	}
}
