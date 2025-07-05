using System;
using System.Collections.Generic;

namespace Narrative.Data
{
	[Serializable]
	public class StageDialogueData
	{
		public int Level;
		public int Stage;
		public List<Replica> Replicas = new List<Replica>();

		public StageDialogueData()
		{
			
		}
	}
}