using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class EnemyState 
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected float stateTimer;

    protected bool triggerCalled;
    private string animBoolName;

    protected Rigidbody2D rb;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;    
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }
    public virtual void Enter()
    {
        triggerCalled = false;
        enemyBase.anim.SetBool(animBoolName, true);
        rb = enemyBase.rb;
    }

    public virtual void Exit() 
    {
        enemyBase.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishedTrigger()
    {
        triggerCalled = true;
    }
}
