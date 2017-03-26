using System;

namespace AssemblyCSharp
{
	public class Player
	{
		public string id;
		public string instrument;

		public Player (string id, String instrument)
		{
			this.id = id;
			this.instrument = instrument;
		}

		public string ToString(){
			return "id: " + id + " instrument: " + instrument;
		}

	}
}

