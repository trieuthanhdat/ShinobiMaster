using UnityEngine;

namespace Game.Enemy
{
	public class EnemyRobot: Enemy
	{
		public Transform hat;
		
		public override void Kill()
		{
			this.hat.GetComponent<Renderer>().material = this.deadMaterial;
		
			base.Kill();
		}
	}
}