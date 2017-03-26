using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class NoteSpawner : MonoBehaviour {

	private Vector3 note_start;
	private Vector3 note_end;
	private Vector3 travelPath;
	private Vector3 travelDelta;
	private GameObject note_object;


	private int note_journey_time = 2000;


	void Awake(){
	}
	// Use this for initialization
	void Start () {
		note_start = transform.Find("Begin").position;
		Debug.Log (note_start);
		note_end = transform.Find("End").position;

		travelPath = note_end - note_start;
		//Debug.Log (travelPath);
		travelDelta = travelPath/ (note_journey_time/1000);

	}

	// Update is called once per frame
	void Update () {
		
	}
		
	void FixedUpdate(){
	}

	public void setJourneyTime(int time){
		this.note_journey_time = time;
		Debug.Log ("Setting time with : " + time);
	}

	public void setNoteObject(GameObject noteObject){
		this.note_object = noteObject;

		travelPath = note_end - note_start;
		//Debug.Log (travelPath);
		travelDelta = travelPath/ (note_journey_time/1000);
	}

	public void spawnNote(){
		Debug.Log ("note spawned");
		GameObject newNote = Instantiate (note_object, note_start, transform.rotation);
		newNote.GetComponent<Rigidbody> ().velocity = travelDelta;
		//spawned_notes.Add (newNote);


	}


}
