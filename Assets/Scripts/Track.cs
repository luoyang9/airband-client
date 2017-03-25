using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;


namespace AssemblyCSharp
{
	[Serializable]
	public class Track
	{
		public String id;
		public String instrument;
		public String name;
		public String thumbnail;
		public String audio_file;
		int length;
		public ArrayList notes;

		public Track (String id, String instrument, String name, String thumbnail, String audio_file,
			int length, ArrayList notes)
		{
			this.id = id;
			this.instrument = instrument;
			this.name = name;
			this.thumbnail = thumbnail;
			this.audio_file = audio_file;
			this.length = length; 
			this.notes = notes;
		}
	}
}

