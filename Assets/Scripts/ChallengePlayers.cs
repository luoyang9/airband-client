using System;

namespace AssemblyCSharp
{
	public class ChallengePlayer
	{
		public string id;
		public bool is_ready;
		public int track_id;
		public int score;

		public ChallengePlayer (string id, bool is_ready, int track_id, int score)
		{
			this.id = id;
			this.is_ready = is_ready;
			this.track_id = track_id;
			this.score = score;
		}
	}
}

