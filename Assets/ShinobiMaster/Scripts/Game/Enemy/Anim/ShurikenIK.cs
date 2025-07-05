using System;
using System.Collections;
using UnityEngine;

namespace Game.Enemy.Anim
{
	enum AnimState
	{
		Idle, Attack
	}

	public class ShurikenIK: MonoBehaviour
	{
		public Action<Transform> OnAttackAnimEnd { get; set; }
	
		private Animator animator;

		private Vector3 handPos;
		[SerializeField] private Transform rightHandAttackPos1;

		private AnimState animState;
		

		private void Awake()
		{
			this.animator = GetComponent<Animator>();
		}


		private void Update()
		{
			if (this.animState == AnimState.Idle)
			{
				this.handPos = Vector3.MoveTowards(this.handPos, rightHandAttackPos1.position, 15f * Time.deltaTime);
			}
		}

		private void OnAnimatorIK(int layerIndex)
		{
			this.animator.SetIKPosition(AvatarIKGoal.RightHand, this.handPos);
			this.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
		}


		public void Attack(Transform target)
		{
			StartCoroutine(AttackProcess(target));
		}

		private IEnumerator AttackProcess(Transform target)
		{
			this.animState = AnimState.Attack;

			var dir = (target.position - this.rightHandAttackPos1.position).normalized;

			dir.z = 0;

			this.handPos = this.rightHandAttackPos1.position;
			
			var startPos = this.handPos;
			var targetPos = this.handPos + dir * 1.5f;

			var t = 0.1f;

			var currT = t;

			while (currT > 0)
			{
				var lerp = 1 - currT / t;
			
				currT -= Time.deltaTime;

				this.handPos = Vector3.Lerp(startPos, targetPos, lerp);

				yield return null;
			}
			
			OnAttackAnimEnd?.Invoke(target);
			
			this.animState = AnimState.Idle;
		}
	}
}