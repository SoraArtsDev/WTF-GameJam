using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    EnemySkeleton enemy;
    private Transform player;
    private int moveDir;

    public SkeletonBattleState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName, EnemySkeleton enemy) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("player").transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(enemy.IsPlayerDetected())
        {
            stateTimer = enemy.aggroTime;
            if(enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                //Debug.Log("Attack");
                if(CanAttack())
                stateMachine.ChangeState(enemy.attackState);
                return;
            }
            else
            {
                if(stateTimer<0 || Vector2.Distance(player.transform.position,enemy.transform.position) > 10)
                {
                    stateMachine.ChangeState(enemy.idleState);
                }
            }
        }

        if(player.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if(player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }
        
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    private bool CanAttack()
    {
        if(Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            enemy.lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
}
