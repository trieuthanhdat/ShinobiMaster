using System.IO;
using UnityEngine;

namespace Narrative.Data
{
	public class DialogueDataController
	{
		private static DialogueDataController instance;
	
		private const string FileName = "DialogueData";
	
		public DialogueData DialogueData { get; set; }
	
	
	
		private DialogueDataController()
		{
			DialogueData = LoadData();
		}



		public static DialogueDataController GetInstance()
		{
			if (instance == null)
			{
				instance = new DialogueDataController();
			}

			return instance;
		}

		private DialogueData LoadData()
		{
			DialogueData dialogueData;
			
			var file = Resources.Load<TextAsset>(FileName);
			
			dialogueData = JsonUtility.FromJson<DialogueData>(file.text);

			return dialogueData;
		}
	}
}