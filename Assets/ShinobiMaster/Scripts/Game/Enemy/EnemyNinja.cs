using System;
using System.Collections;
using Game.Enemy.Anim;
using UnityEngine;

namespace Game.Enemy
{
	public class EnemyNinja : Enemy
	{
		[SerializeField] private Transform[] movePoints;

		private Rigidbody rigidbody;
		
		private NinjaPhysics ninjaPhysics;
		private NinjaAnimationControll animationControll;
		[SerializeField] private ContactPhysics contactPhysics;
		[SerializeField] private GroundDetect groundDetect;
		[SerializeField] private LayerMask groundCheckMask;
		[SerializeField] private float delayBetweenJumps;
		[SerializeField] private JumpExplosion jumpExplosion;

		[SerializeField] private FastIKFabric headIK;
		public ShurikenIK ShurikenIk;
		private bool isFly;
		private bool canMove;

		private Coroutine pointReachedCoroutine;
		
		
		

		protected override void Awake()
		{
			base.Awake();

			this.jumpExplosion = GetComponent<JumpExplosion>();
			this.rigidbody = GetComponent<Rigidbody>();

			this.rigidbody.useGravity = false;
			
			ninjaPhysics = new NinjaPhysics();
			ninjaPhysics.SetRigidbody(rigidbody, GetComponent<CapsuleCollider>(), contactPhysics, groundDetect);

			ninjaPhysics.PlayerConnect += NinjaConnect;
			ninjaPhysics.PlayerStartFly += OnNinjaStartFly;
			
			this.animationControll = new NinjaAnimationControll();
			
			animationControll.SetParams(ninjaPhysics, GetComponentInChildren<Animator>(), this);
		}

		protected override void Start()
		{
			base.Start();

			StartCoroutine(NinjaProcess());

			this.headIK.Target = GameHandler.Singleton.Player.transform;
			
			this.ninjaPhysics.ResetParams();

			this.canMove = true;
		}

		private void FixedUpdate()
		{
			this.rigidbody.velocity += Physics.gravity * 0.25f * Time.fixedDeltaTime;
		}

		private void LateUpdate()
		{
			ninjaPhysics.MoveRunUpdate();
		}




		protected override void Tick(Player target)
		{
			if (this.gettingUpCoroutine == null && !this.lay && !this.isFly)
			{
				attackTarget.UpdateTarget(target, Time.deltaTime);
			}

			if (this.gettingUpCoroutine == null && !this.lay && IsGrounded())
			{
				orientation?.UpdateTarget(target);
			}
		}

		public override void StopMove()
		{
			this.canMove = false;
		}
		
		public override void StartMove()
		{
			this.canMove = true;
		}

		private bool IsGrounded()
		{
			return Physics.Raycast(this.transform.position, -Vector3.up, 0.5f, groundCheckMask);
		}

		private IEnumerator NinjaProcess()
		{
			while (true)
			{
				while (Pause.IsPause)
				{
					yield return null;
				}
			
				for (var i = 0; i < movePoints.Length; i++)
				{
					yield return new WaitForSeconds(this.delayBetweenJumps);
					
					while (Pause.IsPause)
					{
						yield return null;
					}

					while(!this.canMove)
					{
						yield return null;
					}

					var p = i == movePoints.Length - 1 ? 0 : i + 1;
					
					if (movePoints[p] == null)
					{
						break;
					}

					var targetPosition = movePoints[p].position;
					
					var dir = (targetPosition - this.transform.position + Vector3.up * 0.5f).normalized;

					this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
					this.rigidbody.velocity = Vector3.zero;
					this.rigidbody.angularVelocity = Vector3.zero;
					
					var force = 1200f * (targetPosition - this.transform.position + Vector3.up * 0.5f).magnitude / 9f;
					
					this.rigidbody.AddForce(dir * force, ForceMode.Impulse);
					ninjaPhysics.SetStartJump();
					
					this.isFly = true;

					this.weapon.gameObject.SetActive(false);
					this.weaponForRagdoll.gameObject.SetActive(true);
					
					float x = Mathf.Sign((targetPosition - this.transform.position + Vector3.up * 0.5f).x);
					if (x > 0)
					{
						this.transform.eulerAngles = new Vector3(0, 90, 0);
					}
					else
					{
						this.transform.eulerAngles = new Vector3(0, -90, 0);
					}
					
					foreach (var ikFabric in ikFabrics)
					{
						ikFabric.enabled = false;
					}
					
					while (Vector3.Distance(this.transform.position + Vector3.up * 0.5f, movePoints[p].position) > 0.5f)
					{
						while (Pause.IsPause)
						{
							yield return null;
						}
						
						yield return null;
					}
					
					this.pointReachedCoroutine = StartCoroutine(WhenPointReached());
				}

				yield return null;
			}
		}

		protected override void CallKill()
		{
			base.CallKill();
			
			this.weapon.gameObject.SetActive(false);
			this.weaponForRagdoll.gameObject.SetActive(true);

			foreach (var ikFabric in ikFabrics)
			{
				ikFabric.enabled = false;
			}

			if (this.pointReachedCoroutine != null)
			{
				StopCoroutine(this.pointReachedCoroutine);
			}
		}

		private IEnumerator WhenPointReached()
		{
			yield return new WaitForSeconds(0.5f);

			if (IsGrounded())
			{
				foreach (var ikFabric in ikFabrics)
				{
					ikFabric.enabled = true;
				}
				
				this.weapon.gameObject.SetActive(true);
				this.weaponForRagdoll.gameObject.SetActive(false);
			}
			
			this.isFly = false;
		}
		
		private void OnNinjaStartFly()
		{
			this.jumpExplosion.Play();
		}
		
		private void NinjaConnect(Collision collision)
		{
			if (!ninjaPhysics.GetMove())
			{
				Vector3 normal = ninjaPhysics.GetNormalContact();
				float dot = Vector3.Dot(Vector3.up, normal);
				if (dot <= 0.1f && normal.y >= -0.1)
				{
					ninjaPhysics.PlayerStopFly();
				}
				else if (!ninjaPhysics.GetMove())
				{
					if (normal.y >= -0.1)
					{
						ninjaPhysics.PlayerStopFly();
					}
				}
			}
		}
	}
}
