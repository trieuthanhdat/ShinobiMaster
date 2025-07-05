using System.Collections;
using UnityEngine;

namespace Game.Enemy.Weapon
{
	[RequireComponent(typeof(Weapon))]
	public class SinglesShooter: MonoBehaviour, IShooter
	{
		public Weapon Weapon { get; set; }
		public bool IsFiring { get; set; }


		private void Awake()
		{
			Weapon = GetComponent<Weapon>();
		}




		public void Fire(Transform target)
		{
			IsFiring = true;
			Weapon.Fire(target);
			IsFiring = false;
		}
	}
}