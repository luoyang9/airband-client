using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class Note : Object
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

		public string ToString() {
			return "[Note: id="+id+",offset="+offset+",is_pressed="+is_pressed+"]";
		}
	}
}

