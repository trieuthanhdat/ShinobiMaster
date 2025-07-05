using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Advertising;
using Game;
using Game.Chests;
using Game.Enemy;
using Game.EventGame;
using Game.LevelControll;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // 1 param - сам завершил уровень - false. скипнул - true
    public event System.Action<bool> StageComplete;

    [SerializeField] private float widthStage;

    [SerializeField] private Vector2 hegihtStage;

    [SerializeField] private Transform startElevator;

    [SerializeField] private Transform endElevator;
    public Renderer[] Wallstripes;
    public Renderer[] Walls;
    public Renderer[] Floor;
    public Renderer[] ElevatorDoors;
    public Renderer[] ElevatorFrames;
    public Renderer[] Balks;
    public Renderer[] Windows;
    public GameObject Background;

    public Transform BossPlace;
    public Transform NarrativeBossPlace;
    public EnvVisualType EnvVisualType;
    [Space(15)]
    [SerializeField] private StageParamScene stageParam;

    [Space(15)]
    [SerializeField] private StageEvent[] stageEvents;

    public Player PlayerStage { get; private set; }
    public List<Chest> Chests { get; private set; }
    public IBreakable[] Breakables { get; private set; }

    private bool enemyOver;
    private bool stageCompleted;



    private void Awake()
    {
        Breakables = FindInterfaces.Find<IBreakable>().ToArray();
    }

    public Vector2 GetWidth()
    {
        Vector2 vector = new Vector2();
        vector.x = transform.position.z - widthStage / 2f;
        vector.y = transform.position.z + widthStage / 2f;
        return vector;
    }

    public Vector2 GetHeight()
    {
        return hegihtStage;
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        return startElevator.position;
    }

    public void InitStage(Player player)
    {
        Chests = FindObjectsOfType<Chest>().ToList();
        EventManager.OnStageInited?.Invoke(this);
    
        this.PlayerStage = player;
        PlayerStage.gameObject.SetActive(true);
        EnemyControll.Singleton.EnemiesOver += EnemyKillOnStage;
        if (stageParam != null)
            stageParam.SetParams();
    
        PlayerStage.PlayerTeleport(startElevator.position);

        for (int i = 0; i < stageEvents.Length; i++) {
            stageEvents[i].LoadStage(this);
        }
    }

    private void Update()
    {
        if (IsBossDead() && !ReferenceEquals(PlayerStage, null) && !ReferenceEquals(endElevator, null) 
            && !this.stageCompleted && GetDistToEnd() < 2.3f)
        {
            if (StageComplete != null)
                StageComplete(false);
            enemyOver = false;

            this.stageCompleted = true;
        }

        for (int i = 0; i < stageEvents.Length; i++)
        {
            stageEvents[i].UpdateStage();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < stageEvents.Length; i++)
        {
            stageEvents[i].LateUpdateStage();
        }
    }

    private void EnemyKillOnStage()
    {
        enemyOver = true;
    }

    public float GetDistToStart() {
        return Vector3.Distance(PlayerStage.transform.position, startElevator.position);
    }

    public bool GetEnemyOver()
    {
        return enemyOver;
    }
    
    public bool IsBossDead()
    {
        var boss = GameHandler.Singleton.Level.Boss;
    
        return ReferenceEquals(boss, null) || boss.IsDead;
    }

    public float GetDistToEnd()
    {
        return Vector3.Distance(PlayerStage.transform.position, endElevator.position);
    }

    public void DisableStage()
    {
        EnemyControll.Singleton.EnemiesOver -= EnemyKillOnStage;
        for (int i = 0; i < stageEvents.Length; i++)
        {
            stageEvents[i].UnLoadStage();
        }
    }
}
