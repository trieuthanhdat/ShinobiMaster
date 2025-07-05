using UnityEngine;

namespace Game.UI
{
	public class MenuUI: MonoBehaviour
	{
		public GameObject MenuPanel;
	
	
		public void Play()
		{
			if (GameHandler.Singleton.Level.CurrentStage == null)
			{
				GameHandler.Singleton.Level.StartGame();
				MenuPanel.SetActive(false);
			}
			else
			{
				MenuPanel.SetActive(false);
			}
			
			Pause.OffPause();
		}

		public void OpenMenu()
		{
			LevelProgressUI.Singleton.UpdateLevelProgress(GameHandler.Singleton.Level.GetNumLevel() - 1, 
				GameHandler.Singleton.Level.CurrStageNumber);
			MenuPanel.SetActive(true);
			Pause.SetPause();
		}
	}
}