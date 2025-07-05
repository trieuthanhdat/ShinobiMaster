using System;
using System.Linq;
using BzKovSoft.RagdollTemplate.Scripts.Charachter;
using Game.Enemy.Weapon;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        public UnityAction OnDie { get; set; }
        public UnityAction<int> OnTakeDamage { get; set; }
        public Collider Collider { get; protected set; }
        [SerializeField] protected AttackIcon attackIconPrefab;
        [SerializeField] protected Vector3 posAttackIcon;
    
        [SerializeField] protected Transform baseRoot;
        [SerializeField] protected Weapon.Weapon weapon;
        [SerializeField] protected GameObject weaponForRagdoll;
        [SerializeField] protected EntityBlood blood;
        [SerializeField] protected Material deadMaterial;
        [SerializeField] protected Renderer renderer;

        public Transform RightFinger;
        public Sprite EnemyIcon;
        protected Orientation orientation;
        public AttackTarget attackTarget;

        public bool Invincible { get; set; }
        public bool SpineDirToEnemy;

        [field:SerializeField] public int Health { get; set; }

        public AttackIcon IconAttack { get; set; }
   
    
        public Ragdoll Ragdoll { get; private set; }
        protected BzRagdoll bzRagdoll;

        protected Animator animator;

        protected FastIKFabric[] ikFabrics;
    
        protected Coroutine gettingUpCoroutine;

        [SerializeField] protected float layDuration;
        protected bool lay;
        private static readonly int Run = Animator.StringToHash("Run");

        public bool BaseRootActive => this.baseRoot.gameObject.activeInHierarchy;
        public bool NeedUpdateTarget;
        private static readonly int Idle = Animator.StringToHash("Idle");
        public bool IsDead { get; set; }



        protected virtual void Awake()
        {
            NeedUpdateTarget = true;
            Collider = GetComponentInChildren<Collider>();
            OnTakeDamage += OnTakeDamageHandler;

            this.animator = this.baseRoot.GetComponentInChildren<Animator>();
            this.ikFabrics = this.baseRoot.GetComponentsInChildren<FastIKFabric>();
            this.Ragdoll = this.baseRoot.GetComponentInChildren<Ragdoll>();
            this.bzRagdoll = this.baseRoot.GetComponentInChildren<BzRagdoll>();
            
            orientation = new Orientation();
            orientation.Init(this);
            attackTarget.Init(weapon, this, this.weapon.GetComponent<IShooter>());
        }

        protected virtual void Start()
        {
            IconAttack = Instantiate(attackIconPrefab, transform);
            
            IconAttack.transform.localScale = new Vector3(IconAttack.transform.localScale.x/IconAttack.transform.parent.localScale.x,
                IconAttack.transform.localScale.y/IconAttack.transform.parent.localScale.y,
                IconAttack.transform.localScale.z/IconAttack.transform.parent.localScale.z);
                
            IconAttack.transform.localPosition = posAttackIcon;
            
            IconAttack.SetAttackTarget(this.attackTarget);
            
            EnemyControll.Singleton.AddEnemy(this);
        
            this.weaponForRagdoll.SetActive(false);
        }

        protected virtual void Update()
        {
        }
    
        protected virtual void OnDestroy()
        {
            EnemyControll.Singleton?.RemoveEnemy(this);
        
            OnTakeDamage -= OnTakeDamageHandler;
        }

        public void UpdateTick(Player target)
        {
            if (target != null)
                Tick(target);
        }

        protected virtual void Tick(Player target)
        {
            if (NeedUpdateTarget && this.gettingUpCoroutine == null && !this.lay)
            {
                orientation?.UpdateTarget(target);
                attackTarget.UpdateTarget(target, Time.deltaTime);
            }
        }

        public virtual void StopMove()
        {
            
        }

        public virtual void StartMove()
        {
            
        }

        public void TakeDamage(int damage)
        {
            if (Invincible)
            {
                return;
            }
    
            Health -= damage;

            if (Health <= 0)
            {
                Kill();
            }

            OnTakeDamage?.Invoke(damage);
        }

        public Weapon.Weapon GetWeapon()
        {
            return this.weapon;
        }

        public void RunAnim()
        {
            this.animator.SetTrigger(Run);
        }
        
        public void IdleAnim()
        {
            this.animator.SetTrigger(Idle);
        }

        public virtual void Kill()
        {
            IsDead = true;
            IconAttack.gameObject.SetActive(false);
            EnemyControll.Singleton?.RemoveEnemy(this);
            this.renderer.material = this.deadMaterial;
            
            CallKill();
            
            OnDie?.Invoke();
        }

        public Material[] GetMaterials()
        {
            var renderers = GetComponentsInChildren<Renderer>();
        
            return renderers.Select(r => r.material).ToArray();
        }

        public void GetUp()
        {
            this.gameObject.transform.position = new Vector3(this.Ragdoll.Hips.position.x, 
                this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            
            this.Ragdoll.Hips.localPosition = Vector3.zero;

            this.animator.enabled = true;
        
            this.bzRagdoll.GetUp();
        }


        public void GotUp()
        {
            Invincible = false;
        
            this.Ragdoll.ZeroVelocity();
        
            this.weaponForRagdoll.SetActive(false);
            this.weapon.gameObject.SetActive(true);
			
            foreach (var ikFabric in this.ikFabrics)
            {
                ikFabric.enabled = true;
            }
			
            this.gameObject.layer = LayerMask.NameToLayer("Enemy");
            this.Ragdoll.SetMask(LayerMask.NameToLayer("EnemyRagdoll"));
        
            this.gettingUpCoroutine = null;
            this.lay = false;
        }

        protected virtual void CallKill()
        {
            Invoke(nameof(DestroyAll), 3f);
        }
        
        private void DestroyAll()
        {
            Destroy(gameObject);
        }
    
        protected virtual void OnTakeDamageHandler(int damage)
        {
            this.weaponForRagdoll.SetActive(true);
            this.weapon.gameObject.SetActive(false);
    
            blood.ShowBlood();
        
            SetActiveIKFabrics(false);
        
            this.GetComponentInChildren<Animator>().enabled = false;

            this.gameObject.layer = LayerMask.NameToLayer("EnemyDeadParts");
            this.Ragdoll.SetMask(LayerMask.NameToLayer("EnemyDeadParts"));
        }

        public void SetActiveIKFabrics(bool active)
        {
            var ikFabrics = this.baseRoot.GetComponentsInChildren<FastIKFabric>();

            foreach (var ikFabric in ikFabrics)
            {
                ikFabric.enabled = active;
            }
        }
    
    }

    [System.Serializable]
    public class AttackTarget
    {
        public Action OnStartAttack { get; set; }
        public Action OnStopAttack { get; set; }
        public Action<Enemy, Transform> OnAttack { get; set; }
    
        public bool IsAttack { get; private set; }

        public float speedAttack;
        Enemy enemy;
        Weapon.Weapon weapon;
        public bool AttackPlayerIfNear;
        public IShooter Shooter { get; private set; }

        public bool IsReadyToAttack => CurrentStartAttackDelay >= StartAttackDelay;
        
        public float CurrentAttackDelay { get; set; }
        public float CurrentAfterAttackDelay { get; set; }
        [field:SerializeField] public float AfterAttackDelay { get; set; }
        public float StartAttackDelay { get; set; }
        public float CurrentStartAttackDelay { get; set; }
        [field:SerializeField] public float BeforeAttackDelay { get; set; }
        public float CurrentBeforeAttackDelay { get; set; }

        private bool halfAttack;
        public bool needSetWeaponDir;
        private bool attacked;
        
        

        public void Init(Weapon.Weapon weapon, Enemy enemy, IShooter shooter)
        {
            this.enemy = enemy;
            this.weapon = weapon;
            this.Shooter = shooter;
            this.speedAttack = DifficultyRepository.Instance.GetCurrentDifficultyConfig().AttackSpeed;
            CurrentBeforeAttackDelay = BeforeAttackDelay;
            CurrentAfterAttackDelay = AfterAttackDelay;

            if(enemy is EnemyNinja enemyNinja)
            {
                enemyNinja.ShurikenIk.OnAttackAnimEnd += this.Shooter.Fire;
            
                OnAttack += (e, target) =>
                {
                    enemyNinja.ShurikenIk.Attack(target);
                };
            }
            else
            {
                OnAttack += (e, target) =>
                {
                    this.Shooter.Fire(target);
                };
            }

            var attackDelay = DifficultyRepository.Instance.GetCurrentDifficultyConfig().StartAttackDelay;

            StartAttackDelay = Random.Range(attackDelay.x, attackDelay.y);
        }

        public void UpdateTarget(Player target, float duration)
        {
            if (weapon == null)
            {
                return;
            }
        
            var dir = target.transform.position - weapon.transform.position;

            dir.z = 0;
            
            dir.Normalize();
            
            if (enemy.SpineDirToEnemy)
            {
                enemy.Ragdoll.Spine2IK.forward = dir.normalized;
            } 
            else if (!(enemy is EnemyNinja) && this.needSetWeaponDir)
            {
                weapon.SetDirection(dir.normalized);
            }
        
            if (!IsAttack)
            {
                return;
            }

            if (!this.Shooter.IsFiring)
            {
                if (CurrentStartAttackDelay >= StartAttackDelay)
                {
                    CurrentAttackDelay += duration;

                    if (CheckPlayerShot(target))
                    {
                        if (!this.halfAttack && CurrentAttackDelay >= speedAttack * 0.5f)
                        {
                            this.halfAttack = true;
                            
                            AudioManager.Instance.PlayEnemyPreAttackSound();
                        }
                    
                        if (CurrentAttackDelay >= speedAttack)
                        {
                            var dist = (target.transform.position - this.enemy.transform.position).magnitude;

                            if (AttackPlayerIfNear)
                            {
                                if (!this.attacked && CurrentBeforeAttackDelay <= 0)
                                {
                                    OnAttack?.Invoke(enemy, target.transform);

                                    this.attacked = true;
                                }

                                this.needSetWeaponDir = false;
                            }
                            else
                            {
                                if (dist > DifficultyRepository.Instance.GetCurrentDifficultyConfig().AttackCancellationRadius)
                                {
                                    if (!this.attacked && CurrentBeforeAttackDelay <= 0)
                                    {
                                        OnAttack?.Invoke(enemy, target.transform);
                                        
                                        this.attacked = true;
                                    }
                                    
                                    this.needSetWeaponDir = false;
                                }
                            }

                            if (CurrentBeforeAttackDelay <= 0)
                            {
                                if (CurrentAfterAttackDelay <= 0)
                                {
                                    CurrentAttackDelay = 0f;
                                    CurrentStartAttackDelay = 0f;
                                    CurrentBeforeAttackDelay = BeforeAttackDelay;
                                    CurrentAfterAttackDelay = AfterAttackDelay;
                                    this.needSetWeaponDir = true;

                                    this.halfAttack = false;
                                    this.attacked = false;
                                }
                                else
                                {
                                    CurrentAfterAttackDelay -= duration;
                                }
                            }
                            else
                            {
                                CurrentBeforeAttackDelay -= duration;
                            }
                        }
                    }
                }
                else
                {
                    CurrentStartAttackDelay += duration;
                }
            }
        }

        public void StartAttack()
        {
            IsAttack = true;
            OnStartAttack?.Invoke();
        }

        public void StopAttack()
        {
            IsAttack = false;
            CurrentAttackDelay = 0f;
            StartAttackDelay = 0f;
            OnStopAttack?.Invoke();
        }
    

        public bool CheckPlayerShot(Player target)
        {
            RaycastHit hit;
            if (Physics.Raycast(weapon.GetPosFire(), (target.transform.position - weapon.GetPosFire()).normalized,
                out hit, float.MaxValue,
                (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Border")))) ;
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Orientation
    {
        Enemy enemy;

        public void Init(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void UpdateTarget(Player target)
        {
            float x = Mathf.Sign((target.transform.position - enemy.transform.position).x);
            if (x > 0)
            {
                enemy.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                enemy.transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }
    }
}