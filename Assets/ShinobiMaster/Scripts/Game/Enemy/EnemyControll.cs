using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Enemy;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    public static EnemyControll Singleton { get; private set; }

    public event System.Action EnemiesOver;

    public event System.Action EnemyDead;

    [SerializeField] private Player target;

    private List<Enemy> enemies = new List<Enemy>();

    public int EnemiesCount => enemies?.Count(e => !e.IsDead) ?? 0;

    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    public void AddEnemy(Enemy enemy)
    { 
        enemies.Add(enemy);
    }

    private void Update()
    {
        if (!Pause.IsPause)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].UpdateTick(target);
            }
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Remove(enemy))
        {
            if (EnemyDead != null)
                EnemyDead();
            if (CheckNoEnemy())
            {
                if (EnemiesOver != null)
                    EnemiesOver();
            }
        }
    }

    private bool CheckNoEnemy()
    {
        return enemies.Count == 0;
    }

    public Enemy[] GetArray()
    {
        return enemies.ToArray();
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
