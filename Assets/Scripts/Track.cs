using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class Track
	{
		public String id;
		public String instrument;
		public String name;
		public String artist;
		public String thumbnail_file;
		public String audio_file;
		int duration;
		public List<Note> notes;

		public Track (String id, String instrument, String name, String artist, String thumbnail_file, String audio_file,
			int duration, List<Note> notes)
		{
			this.id = id;
			this.instrument = instrument;
			this.name = name;
			this.thumbnail_file = thumbnail_file;
			this.audio_file = audio_file;
			this.duration = duration; 
			this.notes = notes;
		}

		public String ToString() {
			return "[Track: id="+id+", instrument="+instrument+", name="+name+", artist="+artist+", thumbnail_file="+thumbnail_file+", audio_file="+audio_file+", duration="+duration+", notes="+notes+"]";
		}
	}
}

