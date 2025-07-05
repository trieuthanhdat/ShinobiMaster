using Game.Enemy.Shell;
using UnityEngine;

namespace Game.Enemy.Weapon
{
	public class Laser: Weapon
	{
		public override void Fire(Transform target)
		{
			AudioManager.Instance.PlayEnemyAttackSound();
        
			var shell = ShellGenerator.CreateShell(prefabShell, this);

			var dir = GetFireNormalize();

			shell.StartFly(dir);
			CallFire();
            
			OnFire?.Invoke(new Ray(firePoint.position, dir));
		}
	}
}