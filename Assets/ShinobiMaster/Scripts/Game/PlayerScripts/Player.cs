using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Enemy;
using Game.UI;
using Skins;
using UnityEngine;

[Serializable]
public class ColorSchemeDustPoof
{
    public StageColorScheme ColorScheme;
    public GameObject DustPoofPrefab;
}

public class Player : MonoBehaviour
{
    private const string CurrHPKey = "CurrHP";
    public PlayerStateController StateController { get; private set; }
    
    

    private Action<Enemy, bool> OnJumpInEnemy;
    public Action<int> OnHealthChanged;

    [Range(0f, 1000f)]
    [SerializeField] private float forceJumpMax;

    [Range(0f, 1000f)]
    [SerializeField] private float forceJumpMin;

    private HoverJump hoverJump;

    public int MaxHealth;

    public int Health
    {
        get => PlayerPrefs.GetInt(CurrHPKey, MaxHealth);
        set
        {
            PlayerPrefs.SetInt(CurrHPKey, value);
            PlayerPrefs.Save();
            
            OnHealthChanged?.Invoke(value);
        }
    }
    public float InvulnerabilityDuration;
    public float CurrentInvulnerabilityTime { get; set; }

    public PlayerPhysics PlayerPhysics { get; private set; }

    [SerializeField] private Rigidbody rigidbody;

    [SerializeField] private EntityBlood bloodEffect;
    [SerializeField] private JumpExplosion jumpExplosion;
    [SerializeField] private List<ColorSchemeDustPoof> colorSchemesDustPoof;
    private GameObject currentDustPoof;
    [SerializeField] private Transform dustPoofParent;
    [SerializeField] private ContactPhysics contactPhysics;
    public GroundDetect groundDetect;
    [SerializeField] private CapsuleCollider collider;
    public Transform SwordSlot;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    public WeaponVFX weaponVfx;

    [SerializeField] private AnimationMaterial animationMaterial;
    public AnimationControll animationControll;

    [SerializeField] private TakeDamag takeDamag;
    [SerializeField] private CapsuleCollider EnemyAttackCollider;
    public AttackEnemy AttackEnemys { get; private set; }

    [SerializeField] private Transform root;
    [SerializeField] private float timeForSlow;
    [SerializeField] private ParticleSystem jumpInEnemyTrail;
    [SerializeField] private float jumpInEnemyVelocityMultiplier;
    public float ProjectileReflectionRadius;
    public float MinVelDotForReflection;
    public ParticleSystem ProjectileReflectionFX;
    public bool IsInvulnerable { get; set; }

    private Ragdoll ragdoll;
    private Animator animator;

    public bool InvulnerableToBullets { get; set; }
    public Vector3 PlayerDeathPosition { get; set; }

    public bool IsDead { get; private set; }
    [SerializeField] 
    private ParticleSystem heartUpEffect;
    public int LoseCountHP { get; set; }
    public int PickHP { get; set; }
    
    
    

    private void Awake()
    {
        StateController = new PlayerStateController();

        this.ragdoll = GetComponentInChildren<Ragdoll>();
        this.animator = GetComponentInChildren<Animator>();

        InstantiateDustPoof();

        hoverJump = new HoverJump();

        hoverJump.SetForce(forceJumpMin, forceJumpMax);

        PlayerPhysics = new PlayerPhysics();
        PlayerPhysics.SetRigidbody(rigidbody, animationMaterial.PlayerCollider, contactPhysics, groundDetect);

        animationControll = new AnimationControll();


        AttackEnemys = new AttackEnemy();
        AttackEnemys.Init(EnemyAttackCollider, this);

        animationControll.SetParams(this, PlayerPhysics, AttackEnemys, animationMaterial);

        takeDamag.Take += TakeDamage;

        StateController.InitElements(this, PlayerPhysics);
    }

    private void Start()
    {
        ApplySkin(GameHandler.Singleton.SkinService.CurrentCharacterSkin);
        ApplyWeaponSkin(GameHandler.Singleton.SkinService.CurrentWeaponSkin);
        
        OnJumpInEnemy += OnJumpInEnemyHandler;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Invulnerability(InvulnerabilityDuration);
        }
        
        if (!Pause.IsPause && !IsDead && !StateController.PlayerControll)
        {
            UpdateHoverJump();
            AttackEnemys.UpdateAttack();

            var grounded = Physics.Raycast(transform.position, Vector3.down, 1.0f, this.groundDetect.GetMask());
            var detect = this.groundDetect.CheckDetect();
            
            this.animator.SetBool(IsGrounded, 
            (!grounded && this.PlayerPhysics.isStop && detect) || grounded);
        }
        else {
            if (hoverJump.Active)
            {
                hoverJump.Disable();
            }
        }
    }
    

    private void LateUpdate()
    {
        if (!IsDead)
        {
            if (GameHandler.Singleton.Level.CurrStageNumber != 0)
            {
                CameraControll.Singleton.SetPosition(transform.position);
            }
        }
        if (!IsDead && !StateController.PlayerControll)
        {
            if (!Pause.IsPause)
            {
                PlayerPhysics.MoveRunUpdate();
            }
        }
    }

    public void PlayerTeleport(Vector3 vector)
    {
        transform.position = vector;
        PlayerPhysics.ResetParams();
        animationControll.ResetAnimator();

        if (hoverJump.Active)
        {
            hoverJump.Disable();
        }
    }

    public enum SkinChangingMethod
    {
        SharedMesh, // Current: change meshRenderer.sharedMesh
        ReplaceGameObject // New: replace the already setup GameObject skin
    }
    public SkinChangingMethod skinChangingMethod = SkinChangingMethod.SharedMesh;
    public void ApplySkin(CharacterSkin characterSkin)
    {
        switch (skinChangingMethod)
        {
            case SkinChangingMethod.SharedMesh:
                this.meshRenderer.sharedMesh = characterSkin.SkinPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
                this.meshRenderer.material = characterSkin.SkinMat;
                break;
            case SkinChangingMethod.ReplaceGameObject:
                if (animationMaterial != null && animationMaterial.PlayerSkin != null)
                {
                    // Remove old skin GameObject(s)
                    for (int i = animationMaterial.PlayerSkin.childCount - 1; i >= 0; i--)
                    {
                        Destroy(animationMaterial.PlayerSkin.GetChild(i).gameObject);
                    }
                    // Instantiate new skin as child
                    var newSkin = Instantiate(characterSkin.SkinPrefab, animationMaterial.PlayerSkin);
                }
                break;
        }
    }

    public void ApplyWeaponSkin(WeaponSkin weaponSkin)
    {
        if (SwordSlot.childCount > 0)
        {
            Destroy(SwordSlot.GetChild(0).gameObject);
        }
			
        Instantiate(weaponSkin.SkinPrefab, SwordSlot);
        
        weaponVfx.SetWeapon(weaponSkin);
        
    }
    
    public bool JumpingInEnemy { get; private set; }

    private Enemy jumpEnemyTarget;

    private void UpdateHoverJump()
    {
        if (hoverJump.Active)
        {
            if (hoverJump.CheckDisable())
            {
                TimeControll.Singleton.ChangeTimeScaleSmoothly(
                TimeControll.TimeNormal,TimeControll.SlowTime, this.timeForSlow);
                hoverJump.Disable();

                if (PlayerPhysics.GetMove())
                {
                    PlayerPhysics.StopMove();
                }

                if (PlayerPhysics.CheckJumpInDirection(hoverJump.GetTrajectory().normalized) || !PlayerPhysics.CheckGroundContact())
                {
                    this.jumpEnemyTarget = hoverJump.GetEnemyInTrajectory();

                    if (this.jumpEnemyTarget != null)
                    {
                        OnJumpInEnemy?.Invoke(this.jumpEnemyTarget, true);
                    }
                    else
                    {
                        OnJumpInEnemy?.Invoke(null, false);
                    }
                
                    UpdateVelocity(hoverJump.GetTrajectory() * (JumpingInEnemy ? 
                        this.jumpInEnemyVelocityMultiplier : 1f));
                    jumpExplosion.Play();
                    AudioManager.Instance.PlayPlayerJumpSound();
                }

                return;
            }
            
            hoverJump.Update(this);
            
            TimeControll.Singleton.PauseTimeControl();

            if (Time.timeScale > TimeControll.SlowTime)
            {
                Time.timeScale -= Time.deltaTime * 7f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
        else
        {
            if (hoverJump.CheckMouseClickHover(this))
            {
                hoverJump.StartDetectTrajectory();
                
                if (this.jumpEnemyTarget != null)
                {
                    this.jumpEnemyTarget.StartMove();
                }

                JumpingInEnemy = false;
                InvulnerableToBullets = false;
                this.jumpInEnemyTrail.Stop();
            }
        }
    }

    private void OnJumpEnemyDie()
    {
        OnJumpInEnemy?.Invoke(null, false);
    }
    
    private void OnJumpInEnemyHandler(Enemy enemy, bool jump)
    {
        if (jump)
        {
            if (enemy != null)
            {
                if (this.jumpEnemyTarget != null)
                {
                    this.jumpEnemyTarget.StartMove();
                    this.jumpEnemyTarget.OnDie -= OnJumpEnemyDie;
                }
            
                this.jumpEnemyTarget.StopMove();
                this.jumpEnemyTarget.OnDie += OnJumpEnemyDie;
            }
        
            JumpingInEnemy = true;
            InvulnerableToBullets = true;
            this.jumpInEnemyTrail.Play();
        }
        else
        {
            if (this.jumpEnemyTarget != null)
            {
                this.jumpEnemyTarget.StartMove();
            }
            
            UpdateVelocity(PlayerPhysics.GetVelocity() / this.jumpInEnemyVelocityMultiplier);

            JumpingInEnemy = false;
            InvulnerableToBullets = false;
            this.jumpInEnemyTrail.Stop();
        }
    }

    private void UpdateVelocity(Vector3 velocity)
    {
        PlayerPhysics.SetVelocityFly(velocity);
    }

    private void Dead(int damage)
    {
        PlayerDeathPosition = this.transform.position;
    
        GameHandler.Singleton.PlayerDead();
        this.animator.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("EnemyDeadParts");
        this.ragdoll.SetMask(LayerMask.NameToLayer("EnemyDeadParts"));
        
        this.ragdoll.SetForceRigidbody(rigidbody);

        bloodEffect.ShowBlood();
        IsDead = true;
    }
    
    private void InstantiateDustPoof()
    {
        if (this.currentDustPoof != null)
        {
            Destroy(this.currentDustPoof);
        }

        var dustPoofPrefab = this.colorSchemesDustPoof
            .Single(s => s.ColorScheme == GameHandler.Singleton.Level.StageColorScheme).DustPoofPrefab;

        this.currentDustPoof = Instantiate(dustPoofPrefab, this.dustPoofParent);
        
        this.currentDustPoof.transform.localRotation = Quaternion.identity;
        this.currentDustPoof.transform.localPosition = Vector3.zero;
        this.currentDustPoof.transform.localScale = Vector3.one * 0.75f;

        this.jumpExplosion.JumpEffect = this.currentDustPoof.GetComponent<ParticleSystem>();
    }

    private void TakeDamage(int damage)
    {
        if (IsInvulnerable)
        {
            return;
        }
    
        Health -= damage;

        LoseCountHP++;
        
        Development.Vibration.Vibrate(200L);
        
        PlayHeartbreakEffect();
        
        if (Health <= 0)
        {
            Dead(damage);
        }
        else
        {
            Invulnerability(InvulnerabilityDuration);
            BlinkEffect(0.3f, InvulnerabilityDuration);
        }
    }

    private Coroutine invulnerabilityProcess;
    private static readonly int Fade = Shader.PropertyToID("_Fade");

    public void StopInvulnerability()
    {
        IsInvulnerable = false;
    
        if (this.invulnerabilityProcess != null)
        {
            StopCoroutine(this.invulnerabilityProcess);

            this.invulnerabilityProcess = null;
        }
    }

    public void Invulnerability(float duration)
    {
        CurrentInvulnerabilityTime = duration;
    
        if (this.invulnerabilityProcess == null)
        {
            this.invulnerabilityProcess = StartCoroutine(InvulnerabilityProcess());
        }
    }

    public void PlayHeartbreakEffect()
    {
        this.heartUpEffect.Play();
    }
    
    private Coroutine blinkProcess;
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    public void BlinkEffect(float time, float duration)
    {
        if (this.blinkProcess != null)
        {
            StopCoroutine(this.blinkProcess);

            this.blinkProcess = null;
            
            meshRenderer.material.SetFloat(Fade, 1f);
        }
    
        this.blinkProcess = StartCoroutine(BlinkEffectProcess(time, duration));
    }

    private IEnumerator BlinkEffectProcess(float time, float duration)
    {
        var fadeMaterial = meshRenderer.material;

        var i = 0;
        
        while (duration > 0)
        {
            var fadeStart = fadeMaterial.GetFloat(Fade);
            var fadeTarget = i % 2 == 0 ? 0.1f : 1f;
            
            var t = time;
        
            while (t > 0f)
            {
                var lerp = 1 - t / time;

                var fadeLerp = Mathf.Lerp(fadeStart, fadeTarget, lerp);

                fadeMaterial.SetFloat(Fade, fadeLerp);

                t -= Time.deltaTime;
                duration -= Time.deltaTime;

                yield return null;
            }

            i++;
            
            duration -= Time.deltaTime;

            yield return null;
        }
        
        fadeMaterial.SetFloat(Fade, 1f);

        this.blinkProcess = null;
    }

    private IEnumerator InvulnerabilityProcess()
    {
        IsInvulnerable = true;

        while (CurrentInvulnerabilityTime > 0f)
        {
            CurrentInvulnerabilityTime -= Time.deltaTime;
            
            yield return null;
        }

        IsInvulnerable = false;

        this.invulnerabilityProcess = null;
    }

    public void Respawn() {
        this.animator.enabled = true;
        
        animationControll.ResetAnimator();
        PlayerPhysics.ResetParams();
        
        StopInvulnerability();
    
        gameObject.layer = LayerMask.NameToLayer("Player");
        this.ragdoll.SetMask(LayerMask.NameToLayer("PlayerRagdoll"));
        
        IsDead = false;
    }
}
