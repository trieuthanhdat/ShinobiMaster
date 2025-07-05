using System.Collections;
using UnityEngine;

namespace Menu
{
	public class LoadingScreen: MonoBehaviour
	{
		public static bool IsShown = false;
		public float LoadingScreenDuration;
		public GameObject LoadingScreenPanel;
		


		private void Start()
		{
			StartCoroutine(ShowMenu(LoadingScreenDuration));
		}

		private IEnumerator ShowMenu(float duration)
		{
			if (!IsShown)
			{
				yield return new WaitForSeconds(duration);
			}

			LoadingScreenPanel.SetActive(false);
			
			IsShown = true;
		}
	}
}