using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public abstract class Shell : MonoBehaviour
{
    [SerializeField] protected ShellData shellData;
    public ParticleSystem trailPS;
    
    public ShellData Data
    {
        get
        {
            return shellData;
        }
    }

    protected bool isChecked;
    protected bool reflected;
    public float PushForce;


    public void SetParamsRigidbody()
    {
        shellData.Phys.isKinematic = false;
        shellData.Phys.useGravity = false;
        shellData.Phys.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    public void StartFly(Vector3 normalize)
    {
        isChecked = true;
        shellData.Phys.velocity = shellData.Velocity * normalize;
    }

    protected virtual void FixedUpdate()
    {
        if (isChecked)
        {
            RaycastHit hit;
            
            var dist = (GameHandler.Singleton.Player.transform.position - this.transform.position).magnitude;

            this.transform.forward = this.shellData.Phys.velocity;

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
                
                this.trailPS.Stop();

                this.reflected = true;
            }
            
            if (Physics.Raycast(transform.position, this.shellData.Phys.velocity.normalized, out hit, shellData.Phys.velocity.magnitude * Time.fixedDeltaTime * 2f, shellData.Mask))
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

    protected void DestroyShell()
    {
        Destroy(gameObject);
    }

}
