using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTransition : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}

	public void StartTransition() {
		StartCoroutine (Transition ());
	}

	public IEnumerator Transition() {
		float t = 0.0f;
		Vector3 startingPos = transform.position;
		Vector3 startingRot = transform.localEulerAngles;
		while (t < 1.0f)
		{
			t += Time.deltaTime * (Time.timeScale/2.5f);


			transform.position = Vector3.Lerp(startingPos, new Vector3 (0, 1, -2.9f), t);
			transform.localEulerAngles = Vector3.Lerp(startingRot, new Vector3 (14, 0, 0), t);
			yield return 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
