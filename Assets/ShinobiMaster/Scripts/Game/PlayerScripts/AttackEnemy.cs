using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Chests;
using Game.Chests.Items;
using Game.Enemy;
using UnityEngine;

public class AttackEnemy
{
    public event System.Action PlayerAttack;

    Player player;
    CapsuleCollider capsule;
 
 
 
    public void Init(CapsuleCollider capsule, Player player)
    {
        this.player = player;
        this.capsule = capsule;
    }

    public void UpdateAttack()
    {
        Vector3 start = player.transform.position + new Vector3(0, ((capsule.height - 1) / 2f), 0);
        Vector3 end = player.transform.position - new Vector3(0, ((capsule.height - 1) / 2f), 0);

        if (Physics.CheckCapsule(start, end, capsule.radius, LayerMask.GetMask("Enemy")))
        {
            Enemy[] enemies = EnemyControll.Singleton.GetArray();
            float minDist = float.MaxValue;
            int index = 0;
            for (int i = 0; i < enemies.Length; i++)
            {
                float dist = Vector3.Distance(enemies[i].transform.position, player.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            if (minDist != float.MaxValue)
            {
                var enemy = enemies[index];

                if (Physics.Linecast(this.player.PlayerPhysics.GetCollider().bounds.center, enemy.Collider.bounds.center, LayerMask.GetMask("Map")))
                {
                    return;
                }
            
                PlayerAttack?.Invoke();

                AudioManager.Instance.PlayPlayerAttackSound();
                
                enemy.Ragdoll.AddForce((enemy.transform.position + Vector3.up * 0.8f - this.player.transform.position).normalized * 100f);
                
                enemy.TakeDamage(1);
            }
        }
        
        if (Physics.CheckCapsule(start, end, capsule.radius, LayerMask.GetMask("Chest")))
        {
            var chests = GameHandler.Singleton.Level.CurrentStage.Chests.Where(ch => !ch.IsOpened).ToList();
        
            float minDist = float.MaxValue;
            int index = 0;
            for (int i = 0; i < chests.Count; i++)
            {
                if (chests[i] == null)
                {
                    continue;
                }
            
                float dist = Vector3.Distance(chests[i].transform.position, player.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }

            var chest = chests[index];
            
            if (chest is IBreakable breakableChest && !chest.IsOpened && !breakableChest.IsBroken)
            {
                var haveCenter = chest as IHaveCenter;

                var chestCenter = haveCenter?.GetCenter() ?? chest.transform.position;
            
                if (Physics.Linecast(this.player.PlayerPhysics.GetCollider().bounds.center, chestCenter, LayerMask.GetMask("Map")))
                {
                    return;
                }
                
                PlayerAttack?.Invoke();

                AudioManager.Instance.PlayPlayerAttackSound();
                Development.Vibration.Vibrate(20L);

                breakableChest.Break();
            }
        }
        
        if (Physics.CheckCapsule(start, end, capsule.radius, LayerMask.GetMask("Breakable")))
        {
            var breakables = GameHandler.Singleton.Level.CurrentStage.Breakables.Where(b => !b.IsBroken).ToArray();
        
            var minDist = float.MaxValue;
            var index = 0;
            
            for (var i = 0; i < breakables.Length; i++)
            {
                var comp = (Component) breakables[i];
            
                if (breakables[i] == null || comp == null) continue;
            
                var dist = Vector3.Distance(comp.transform.position, player.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }

            var breakable = breakables[index];

            if (!breakable.IsBroken)
            {
                var comp = (Component) breakable;

                if (comp == null)
                {
                    return;
                }

                var haveCenter = breakable as IHaveCenter;

                var breakableCenter = haveCenter?.GetCenter() ?? comp.transform.position;
                
                if (Physics.Linecast(this.player.PlayerPhysics.GetCollider().bounds.center, breakableCenter, LayerMask.GetMask("Map")))
                {
                    return;
                }
            
                PlayerAttack?.Invoke();

                AudioManager.Instance.PlayPlayerAttackSound();
                Development.Vibration.Vibrate(20L);
            
                breakable.Break();
            }
        }
    }
}
