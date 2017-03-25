using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class NoteDisplay : MonoBehaviour {

	public GameObject[] note_spawns;
	public GameObject note_object;

	private NoteSpawner[] note_spawners;
	private Track track;
	private ArrayList track_note;

	private bool playing = false;
	private float startTime;
	private int noteIdx;

	public int journeyTime = 1000;

	void Awake(){
		
	}

	// Use this for initialization
	void Start () {

		note_spawners = new NoteSpawner[note_spawns.Length];

		for (int x = 0; x < note_spawns.Length; x++) {
			note_spawners [x] = (NoteSpawner)note_spawns [x].GetComponent (typeof(NoteSpawner));
			note_spawners [x].setJourneyTime (this.journeyTime);
			note_spawners [x].setNoteObject (this.note_object);
		}

		Note note1 = new Note (0, 5000, true);
		Note note2 = new Note (0, 7000, true);
		Note note3 = new Note (0, 8000, true);
		Note note4 = new Note (0, 10000, true);

		ArrayList notelist = new ArrayList ();
		notelist.Add (note1);
		notelist.Add (note2);
		notelist.Add (note3);
		notelist.Add (note4);

		Track testTrack = new Track ("abc", "Guitar", "hello", "whatever.jpg", "abc.mp3", 1000,
			                  notelist);
		this.setTrack (testTrack);
		this.trackStart ();

	}



	// Update is called once per frame
	void Update () {
		if (playing) {
			if (noteIdx >= track_note.Count) {
				this.trackStop();
				return;
			}

			float offsetTime = Time.realtimeSinceStartup * 1000 - startTime;

			Note nextNote = (Note)track_note [noteIdx];

			if (offsetTime > nextNote.offset) {	// Time to play the next note
				noteIdx ++;
				note_spawners [nextNote.id].spawnNote ();
			}
		}
	}
	void setTrack(Track track){
		this.track = track;
		track_note = track.notes;
	}

	void trackStart(){
		Debug.Assert (this.track != null);

		startTime = Time.realtimeSinceStartup;
		noteIdx = 0;
		playing = true;
	}

	void trackStop(){
		playing = false;
		// @Todo: clear up unused assets
	}
}
