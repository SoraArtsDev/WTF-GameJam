using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected int moveSpeed;
    [SerializeField] protected LayerMask whatIsGround;
    
    public Rigidbody2D rb { get;private set; }

    public Animator anim { get; private set; }

    public int facingDir = 1;
    protected bool facingRight = true;

    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();
    }

    public virtual void ZeroVelocity() => rb.velocity = new Vector2 (0,0);
    public virtual void SetVelocity(float xVelocity,float yVelocity)
    {
        rb.velocity = new Vector2 (xVelocity,yVelocity);
    }
   public void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void FlipController()
    {
        if(rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if(rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    public virtual bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

}
