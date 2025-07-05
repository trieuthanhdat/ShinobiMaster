using UnityEngine;

namespace Game.Enemy.Weapon
{
	public interface IShooter
	{
		Weapon Weapon { get; set; }
		
		bool IsFiring { get; set; }
	
		void Fire(Transform target);
	}
}