using UnityEngine;

namespace Game.Enemy.Weapon
{
	public class RaycastWeapon: Weapon
	{
		[field:SerializeField] public float Distance { get; set; }
		public int PushForce;
	
	
	
		public override void Fire(Transform target)
		{
			AudioManager.Instance.PlayEnemyAttackSound();

			var dir = (target.position - firePoint.position).normalized;
        
			CallFire();

			var ray = new Ray(firePoint.position, this.firePoint.forward);

			if (Physics.Raycast(ray, out var hit, Distance, 1 << LayerMask.NameToLayer("Player")))
			{
				ShotResolver.Resolver(hit, PushForce);
			}
			
			OnFire?.Invoke(new Ray(firePoint.position, dir));
		}
	}
}