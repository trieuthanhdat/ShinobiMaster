using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SkipLevelUI: MonoBehaviour
	{
		public static SkipLevelUI Instance;

		[SerializeField] private GameObject panel;
		[field:SerializeField] public Button ClosePanelButton { get; set; }
		[field:SerializeField] public Button SkipLevelButton { get; set; }



		private void Awake()
		{
			Instance = this;
		}

		public void ShowPanel()
		{
			ClosePanelButton.onClick.AddListener(HidePanel);
		
			this.panel.SetActive(true);
			
			StartCoroutine(ShowCloseButton());
		}

		public void HidePanel()
		{
			this.panel.SetActive(false);
			
			SkipLevelButton.onClick.RemoveAllListeners();
			ClosePanelButton.onClick.RemoveAllListeners();
			ClosePanelButton.gameObject.SetActive(false);
		}

		private IEnumerator ShowCloseButton()
		{
			yield return new WaitForSecondsRealtime(2.0f);
			
			ClosePanelButton.gameObject.SetActive(true);
		}
	}
}