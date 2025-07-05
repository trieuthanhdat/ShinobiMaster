using System;
using System.Collections.Generic;

namespace Narrative.Data
{
	[Serializable]
	public class DialogueData
	{
		public List<StageDialogueData> StagesDialogueData = new List<StageDialogueData>();
	
		public DialogueData()
		{
			
		}
	}
}