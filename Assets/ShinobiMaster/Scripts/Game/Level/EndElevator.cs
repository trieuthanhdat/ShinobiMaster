using System.Collections;
using System.Collections.Generic;
using Game.Enemy;
using Game.UI;
using UnityEngine;

[ExecuteInEditMode]
public class EndElevator : StageEvent
{
    private Enemy Boss;
    public event System.Action PlayerExitFromStage;

    [SerializeField] private Vector2 zoneZ;
    [SerializeField] private float posZoneY;
    [SerializeField] private float maxRadiusToZone;

    [SerializeField] private Transform elevatorEnd;

    [Space(15)]
    [SerializeField] private float delayToStartElevator;

    public Transform elevator;
    public Vector3 bossStartPos { get; private set; }
    public Vector3 bossPosOnElevator { get; private set; }
    private Vector3 bossEndPos;
    [SerializeField] private Vector3 startLocalPos;

    [SerializeField] private Vector3 endLocalPos;

    [SerializeField] private Vector3 playerPosOnElevator;

    [SerializeField] private float timeUp;

    bool start;

    Coroutine anim;

    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    protected override void Awake()
    {
        base.Awake();

        this.bossStartPos = new Vector3(this.startLocalPos.x, this.startLocalPos.y, this.startLocalPos.z + 0f);
        this.bossEndPos = new Vector3(this.startLocalPos.x, this.startLocalPos.y, 0.0823f);
        this.bossPosOnElevator = Vector3.zero;
    }

    protected override void CallLoadStage()
    {

    }

    public void SetBoss(Enemy enemy)
    {
        elevator.localPosition = this.bossStartPos;
    
        Boss = enemy;
        
        Boss.transform.position = elevator.TransformPoint(bossPosOnElevator);
    }

    public IEnumerator BossRun(float time)
    {
        var t = time;

        while (t > 0)
        {
            var lerp = 1 - t / time;
        
            t -= Time.deltaTime;

            Boss.transform.position = elevator.TransformPoint(bossPosOnElevator);
            elevator.localPosition = Vector3.Lerp(this.bossStartPos, this.bossEndPos, lerp);

            yield return null;
        }
        
        Destroy(Boss.gameObject);
        
        var elevatorDownTime = 0.7f;

        var currElevatorDownTime = elevatorDownTime;

        while (currElevatorDownTime > 0)
        {
            var lerp = 1 - currElevatorDownTime / elevatorDownTime;
        
            currElevatorDownTime -= Time.deltaTime;
            
            elevator.localPosition = Vector3.Lerp(this.bossEndPos, this.startLocalPos, lerp);

            yield return null;
        }

        elevator.localPosition = this.startLocalPos;
    }

    protected override void CallUpdateStage()
    {
        if (stage.PlayerStage != null && !start && !stage.PlayerStage.IsDead && stage.IsBossDead())
        {
            Transform transformP = stage.PlayerStage.transform;
            if (transformP.position.x > zoneZ.x && transformP.position.x < zoneZ.y)
            {
                ContactPlayer contacts = stage.PlayerStage.StateController.GetContacts();
                if (Mathf.Abs(posZoneY - transformP.position.y) < maxRadiusToZone && (contacts.Left || contacts.Right || contacts.Down))
                {
                    anim = StartCoroutine(AnimMoveToEndElevator(contacts));
                    start = true;
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3((this.zoneZ.y + this.zoneZ.x) * 0.5f, this.posZoneY, 0), 
            new Vector3((this.zoneZ.y - this.zoneZ.x), this.maxRadiusToZone * 2f, 1f));
    }

    IEnumerator AnimMoveToEndElevator(ContactPlayer contacts)
    {
        PlayerExitFromStage?.Invoke();
        
        stage.PlayerStage.StateController.PlayerDirection(-1);

        stage.PlayerStage.StopInvulnerability();
        stage.PlayerStage.IsInvulnerable = true;
        
        TimeControll.Singleton.PauseTimeControl();
        TimeControll.Singleton.ChannelTimeTo(TimeControll.TimeNormal);
        stage.PlayerStage.StateController.BeginControll();

        if (!Physics.Raycast(stage.PlayerStage.transform.position, Vector3.down, 0.905f,
            1 << LayerMask.NameToLayer("Map")))
        {
            Physics.Raycast(stage.PlayerStage.transform.position, Vector3.down, out var hit,
                100,1 << LayerMask.NameToLayer("Map"));
        
            while (stage.PlayerStage.transform.position.y > hit.point.y + 0.905f)
            {
                stage.PlayerStage.transform.position -= Vector3.up * 8f * Time.deltaTime;

                yield return null;
            }
        }
        
        stage.PlayerStage.animationControll.ResetAnimator();

        stage.PlayerStage.StateController.PlayerRun();

        while (stage.PlayerStage.transform.position.x < elevatorEnd.position.x)
        {
            stage.PlayerStage.transform.position += stage.PlayerStage.transform.right * 10 * Time.deltaTime;
        
            yield return null; 
        }
        
        stage.PlayerStage.transform.position = new Vector3(elevatorEnd.position.x, 
            stage.PlayerStage.transform.position.y, stage.PlayerStage.transform.position.z);
            
        stage.PlayerStage.animationControll.animationMaterial.AnimatorPlayer.SetBool(IsGrounded, 
            true);
        stage.PlayerStage.StateController.PlayerStay();

        yield return new WaitForSecondsRealtime(delayToStartElevator);

        var t = timeUp;

        while(t > 0f)
        {
            var lerp = 1 - t / timeUp;
        
            elevator.transform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, lerp);
            stage.PlayerStage.StateController.SetPosition(elevator.TransformPoint(playerPosOnElevator));

            t -= Time.deltaTime;
            
            yield return null;
        }

        stage.PlayerStage.StateController.EndControll();

        anim = null;

        stage.PlayerStage.IsInvulnerable = false;
    }

    protected override void CallUnLoadStage()
    {

    }
}
