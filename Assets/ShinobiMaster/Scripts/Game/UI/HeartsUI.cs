using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
	public class HeartsUI: MonoBehaviour
	{
		public Player Player;
		[SerializeField]
		private HeartbreakUI heartbreakUiPrefab;
		private List<HeartbreakUI> heartbreaks;
		private Coroutine hideWithDelayCoroutine;
		public bool OnHealthChangedHandlerActive { get; set; }
		
		
		

		private void Awake()
		{
			this.heartbreaks = new List<HeartbreakUI>();
		
			for (var i = 0; i < Player.MaxHealth; i++)
			{
				var heartbreakUi = Instantiate(this.heartbreakUiPrefab, this.transform);
				
				this.heartbreaks.Add(heartbreakUi);
			}
		}

		private void Start()
		{
			Player.OnHealthChanged += OnHealthChanged;
			
			SetHeartbreaksVisibility(false);
			ArrangeHeartbreaks(Player.Health);
			UpdateHeartbreaksVisibility(Player.Health);
		}

		private void OnDestroy()
		{
			Player.OnHealthChanged -= OnHealthChanged;
		}


		public void UpdateHeartbreaksVisibility(int health)
		{
			for (var i = 0; i < Player.MaxHealth; i++)
			{
				this.heartbreaks[i].SetHeartVisibility(i < health);
			}
		}

		public void SetHeartbreaksVisibility(bool visible)
		{
			foreach (var heartbreak in this.heartbreaks)
			{
				heartbreak.gameObject.SetActive(visible);
			}
		}
		
		private void PlayHeartbreak(int idx)
		{
			this.heartbreaks[idx].PlayHeartbreak();
		}

		private void ArrangeHeartbreaks(int health)
		{
			var offset = ((health-1) * 5f) / 2;
		
			for (var i = 0; i < health; i++)
			{
				this.heartbreaks[i].transform.localPosition = new Vector3(i * 5f - offset, 0f, 0f);
			}
		}

		public int VisibleHeartsCount()
		{
			return this.heartbreaks.Count(h => h.IsVisible());
		}

		private void OnHealthChanged(int health)
		{
			if (!OnHealthChangedHandlerActive)
			{
				ArrangeHeartbreaks(health);
				UpdateHeartbreaksVisibility(health);
			
				return;
			}
		
			SetHeartbreaksVisibility(true);

			if (health == 0)
			{
				ArrangeHeartbreaks(health);
				UpdateHeartbreaksVisibility(health);
			}
			else
			{
				var visibleHeartsCount = VisibleHeartsCount();

				// если здоровье прибавилось
				if (health > visibleHeartsCount)
				{
					foreach (var heartbreak in this.heartbreaks)
					{
						heartbreak.StopAnim();
					}
				
					ArrangeHeartbreaks(health);
					UpdateHeartbreaksVisibility(health);
				}
				else // если здоровье убавилось
				{
					var heartIdx = 0;

					PlayHeartbreak(heartIdx);

					var lastHeart = this.heartbreaks[this.heartbreaks.Count - 1];

					this.heartbreaks[this.heartbreaks.Count - 1] = this.heartbreaks[heartIdx];

					this.heartbreaks[heartIdx] = lastHeart;
				}
			}

			if (this.hideWithDelayCoroutine != null)
			{
				StopCoroutine(this.hideWithDelayCoroutine);

				this.hideWithDelayCoroutine = null;
			}

			this.hideWithDelayCoroutine = StartCoroutine(HideWithDelay(1.3f));
		}

		private IEnumerator HideWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			
			SetHeartbreaksVisibility(false);
			
			ArrangeHeartbreaks(Player.Health);

			this.hideWithDelayCoroutine = null;
		}
	}
}