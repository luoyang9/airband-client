using System;

namespace AssemblyCSharp
{
	public class Note
	{
		public int id;
		public int offset;
		public bool is_pressed;

		public Note (int id, int offset, bool is_pressed)
		{
			this.id = id;
			this.offset = offset;
			this.is_pressed = is_pressed;
		}
	}
}

