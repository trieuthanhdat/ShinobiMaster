using UnityEngine;

namespace Game.Enemy.Shell
{
	public class LaserShell: global::Shell
	{
		[field:SerializeField]	public TrailRenderer TrailRenderer { get; set; }


		protected override void FixedUpdate()
		{
			if (isChecked)
			{
				this.transform.forward = this.shellData.Phys.velocity;
			
				if (TrailRenderer.positionCount > 0)
				{
					var dist = (GameHandler.Singleton.Player.transform.position - this.transform.position).magnitude;

					var dot = Vector3.Dot(this.shellData.Phys.velocity.normalized,
						GameHandler.Singleton.Player.PlayerPhysics.GetVelocity().normalized);

					if (!this.reflected && dist <= GameHandler.Singleton.Player.ProjectileReflectionRadius 
					                    && dot < GameHandler.Singleton.Player.MinVelDotForReflection)
					{
						GameHandler.Singleton.Player.animationControll.AttackPlayer();
						GameHandler.Singleton.Player.weaponVfx.ShowAnim();
						GameHandler.Singleton.Player.ProjectileReflectionFX.Play();
                
						var newVelDir = -this.shellData.Phys.velocity.normalized;

						this.shellData.Phys.velocity = Vector3.zero;
						this.shellData.Phys.useGravity = true;
						this.shellData.Phys.AddForce(newVelDir * 15, ForceMode.Impulse);
						this.shellData.Mask -= LayerMask.NameToLayer("Player");

						this.reflected = true;

						TrailRenderer.enabled = false;
					}
				
					var startPos = TrailRenderer.GetPosition(0);
					var endPos = TrailRenderer.GetPosition(TrailRenderer.positionCount - 1);

					if (Physics.Linecast(startPos, endPos, out var hit, shellData.Mask))
					{
						var player = hit.transform.GetComponent<Player>();

						if (player != null && player.InvulnerableToBullets)
						{
							this.shellData.Phys.velocity = -this.shellData.Phys.velocity;
                
							var vel = this.shellData.Phys.velocity;

							var angle = Random.value > 0.5f ? 30 : -30;

							var newVel = 
								new Vector3(
									vel.x * Mathf.Cos(angle * Mathf.Deg2Rad)  - vel.y * Mathf.Sin(angle * Mathf.Deg2Rad),
									vel.x * Mathf.Sin(angle * Mathf.Deg2Rad) + vel.y * Mathf.Cos(angle * Mathf.Deg2Rad), 
									vel.z);

							this.shellData.Phys.velocity = newVel * 3f;

							this.reflected = true;
						}
						else
						{
							if (!this.reflected)
							{
								ShotResolver.Resolver(hit, PushForce);
							}

							DestroyShell();
						}
					}
				}
			}
		}
	}
}