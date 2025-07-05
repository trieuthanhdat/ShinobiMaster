using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class BossUI: MonoBehaviour
	{
		public static BossUI Instance;
	
		public Enemy.Enemy Boss
		{
			get => this.boss;
			set
			{
				boss = value;
				
				boss.OnTakeDamage += OnTakeDamage;

				bossStartHealth = boss.Health;
				
				UpdateHealthbar();
			}
		}

		private Enemy.Enemy boss;

		public Image HealthbarImage;

		private int bossStartHealth;




		private void Awake()
		{
			if (Instance)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Instance = this;
			}
		}




		public void SetActive(bool active)
		{
			HealthbarImage.transform.parent.gameObject.SetActive(active);
		}




		private void OnTakeDamage(int damage)
		{
			UpdateHealthbar();
		}

		private void UpdateHealthbar()
		{
			HealthbarImage.fillAmount = Boss.Health / (float) bossStartHealth;
		}
	}
}