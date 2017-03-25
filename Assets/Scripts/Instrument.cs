using System;

namespace AssemblyCSharp
{
	abstract class Instrument
	{

		public string instrument_name;
		public Instrument (String instrument_name)
		{
			this.instrument_name = instrument_name;
			// Read in files and store them
		}

		abstract public void playNote (int id, bool is_pressed);


	}
}

