using UnityEngine;

namespace Game.Enemy.Anim
{
	public class GetUpBehaviour: StateMachineBehaviour
	{
		private Enemy enemy;
	
	
	
	
		public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			if (this.enemy == null)
			{
				this.enemy = animator.GetComponentInParent<Enemy>();
			}

			if (this.enemy != null)
			{
				this.enemy.GotUp();
			}
		}
	}
}