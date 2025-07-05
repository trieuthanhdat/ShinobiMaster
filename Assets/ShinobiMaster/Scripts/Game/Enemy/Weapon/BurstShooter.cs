using System.Collections;
using UnityEngine;

namespace Game.Enemy.Weapon
{
	[RequireComponent(typeof(Weapon))]
	public class BurstShooter: MonoBehaviour, IShooter
	{
		public Weapon Weapon { get; set; }
		public bool IsFiring { get; set; }

		[field:SerializeField] public float DelayBetweenShots { get; set; }
		[field:SerializeField] public int ShotsCount { get; set; }
		
		


		private void Awake()
		{
			Weapon = GetComponent<Weapon>();
		}




		public void Fire(Transform target)
		{
			IsFiring = true;
		
			StartCoroutine(ShootProcess(target));
		}




		private IEnumerator ShootProcess(Transform target)
		{
			for (var i = 0; i < ShotsCount; i++)
			{
				Weapon.Fire(target);

				yield return new WaitForSeconds(DelayBetweenShots);
			}
			
			IsFiring = false;
		}
	}
}