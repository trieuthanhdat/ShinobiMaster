using System.Collections;
using UnityEngine;

namespace Game.UI
{
	public class KeysUI: MonoBehaviour
	{
		[SerializeField] private KeyUI[] keysUI;
		[SerializeField] private float updateStateDelay;



		private void Awake()
		{
			GameHandler.Singleton.PlayerProfile.OnKeysCountChanged += OnKeysCountChanged;
		}

		private void Start()
		{
			UpdateState(GameHandler.Singleton.PlayerProfile.KeysCount, 0);
		}

		private void OnDestroy()
		{
			GameHandler.Singleton.PlayerProfile.OnKeysCountChanged -= OnKeysCountChanged;
		}




		private void OnKeysCountChanged(int keysCount, int prevKeyCount)
		{
			if (this.gameObject.activeInHierarchy && this.updateStateDelay > 0f)
			{
				StartCoroutine(UpdateStateWithDelay(this.updateStateDelay, keysCount, prevKeyCount));
			}
			else
			{
				UpdateState(keysCount, prevKeyCount);
			}
		}

		private IEnumerator UpdateStateWithDelay(float delay, int keysCount, int prevKeyCount)
		{
			yield return new WaitForSecondsRealtime(delay);
			
			UpdateState(keysCount, prevKeyCount);
		}

		private void UpdateState(int keysCount, int prevKeyCount)
		{
			if (keysCount > prevKeyCount)
			{
				for (var i = prevKeyCount; i < keysCount; i++)
				{
					keysUI[i].SetKey();
				}
			}
			else
			{
				for (var i = prevKeyCount - 1; i >= keysCount; i--)
				{
					keysUI[i].SetNoKey();
				}
			}
		}
	}
}