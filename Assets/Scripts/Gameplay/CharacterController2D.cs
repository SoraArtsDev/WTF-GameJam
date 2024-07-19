using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{

    private const float DEAD_ZONE = 0.27f;
    public enum EGravityScale
    {
        ENORMAL,
        EJUMPCUT,
        EHANGTIME,
        ELANDTIME,
        EFALL,
        EDASH,
    };

    public CharacterControllerData controllerData;

    #region Variables

    public float hitDistance;
    public float hitOffset;
    public EGravityScale gravityScale;

    private float lastOnGroundTime;
    private float lastJumpPressedTime;
    private float lastDashPressedTime;
    private float dashWaitTime;

    //Components
    public Rigidbody2D CharacterRigidBody { get; private set; }

    //Jump
    private bool isJumping;
    private bool isGrounded;
    private bool isJumpCut;
    private bool isDashing;
    private bool isJumpFalling;
    private bool isJumpPressed;
    private bool isJumpReleased;
    private bool isDashPressed;
    private bool isDashEnabled;
    private bool isRunning;
    private bool isIdle;

    public bool IsFacingRight { get; private set; }

    public bool IsDashing() { return isDashing; }
    public bool IsJumping() { return isJumping; }
    public bool IsRunning() { return isRunning; }
    public bool IsIdle() { return isIdle; }

    //input
    public InputMap inputMap { get; private set; }
    private Vector2 moveInput;
    private Vector2 dashInput;
    #endregion


    //callbacks;
    public delegate void CallBack();
    public CallBack DashPressed { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        CharacterRigidBody = GetComponent<Rigidbody2D>();

        inputMap = new InputMap();
        inputMap.PlayerController.Enable();
        inputMap.PlayerController.Jump.started += OnJump;
        inputMap.PlayerController.Dash.started += OnDash;

        IsFacingRight = true;
        isJumping = false;
        isDashing = false;
        isGrounded = true;
        isDashPressed = false;
        isJumpPressed = false;
        isJumpReleased = false;
        isDashEnabled = true;

        isRunning = false; ;
        isIdle = true;

        gravityScale = EGravityScale.ENORMAL;
        lastOnGroundTime = 0.0f;
        lastJumpPressedTime = 0.0f;
    }


    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
        lastJumpPressedTime = controllerData.jumpPressBufferTime;
    }

    void OnDash(InputAction.CallbackContext context)
    {
        isDashPressed = true;
        lastDashPressedTime = controllerData.dashPressedBufferTime;
    }

    // Update is called once per frame
    void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;
        lastDashPressedTime -= Time.deltaTime;

        CheckIfShouldDisableDash();

        dashInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        CheckIfInDeadZone();
        moveInput = inputMap.PlayerController.Movement.ReadValue<Vector2>();
        if (moveInput.x != 0)
            SwitchPlayerDirection(moveInput.x > 0);

        isJumpReleased = inputMap.PlayerController.Jump.WasReleasedThisFrame();


#if UNITY_EDITOR
        Vector3 lineStart = transform.position + Vector3.down * hitOffset;
        Vector3 lineEnd = lineStart + Vector3.down * hitDistance;
        Debug.DrawLine(lineStart, lineEnd, Color.red);
#endif

        //Check if Grounded
        if (!isJumping)
        {
            if (CheckIfGrounded())
            {
                //we just landed
                if (lastOnGroundTime < 0.1f)
                {
                    //tell animator to play land animation
                }

                lastOnGroundTime = controllerData.coyoteTime;
                isDashEnabled = true;
            }
        }


        //Check if jump was released earlier
        if (isJumping && isJumpReleased && CharacterRigidBody.velocity.y > 0)
        {
            isJumpCut = true;
        }

        //Check if we started falling
        if (isJumping && CharacterRigidBody.velocity.y < 0)
        {
            isJumpFalling = true;
            isJumping = false;
        }
        if (lastOnGroundTime > 0.0f && !isJumping)
        {
            isJumpCut = false;
            isJumpFalling = false;
        }

        #region Falling Gravity Scale
        if (isDashing)
        {
            gravityScale = EGravityScale.EDASH;
            SetGravityScale(controllerData.gravityScale * controllerData.dashGravityMultiplier);
        }
        //we give priority to soft landing
        else if (controllerData.softLanding && (isJumping || isJumpFalling) && CharacterRigidBody.velocity.y < controllerData.jumpLandTimeThreshold)
        {
            gravityScale = EGravityScale.ELANDTIME;
            SetGravityScale(controllerData.gravityScale * controllerData.jumpLandGravityMultiplier);
        }
        else if (isJumpCut)
        {
            gravityScale = EGravityScale.EJUMPCUT;
            SetGravityScale(controllerData.gravityScale * controllerData.jumpCutGravityMultiplier);
            CharacterRigidBody.velocity = new Vector2(CharacterRigidBody.velocity.x, Mathf.Max(CharacterRigidBody.velocity.y, -controllerData.maxFallSpeed));
        }
        else if ((isJumping || isJumpFalling) && Mathf.Abs(CharacterRigidBody.velocity.y) < controllerData.jumpHangTimeThreshold)
        {
            gravityScale = EGravityScale.EHANGTIME;
            SetGravityScale(controllerData.gravityScale * controllerData.jumpHangGravityMultiplier);
        }
        else if (CharacterRigidBody.velocity.y < 0)
        {
            gravityScale = EGravityScale.EFALL;
            SetGravityScale(controllerData.gravityScale * controllerData.fallGravityMult);
            CharacterRigidBody.velocity = new Vector2(CharacterRigidBody.velocity.x, Mathf.Max(CharacterRigidBody.velocity.y, -controllerData.maxFallSpeed));

        }
        else
        {
            gravityScale = EGravityScale.ENORMAL;
            SetGravityScale(controllerData.gravityScale);
        }

        isRunning = Mathf.Abs(CharacterRigidBody.velocity.x) > 0;
        isIdle = !isRunning && !isJumping && !isJumpFalling && !isJumpCut && lastOnGroundTime > 0.1f;
        #endregion
    }

    void CheckIfInDeadZone()
    {
        dashInput.x = Mathf.Abs(dashInput.x) < DEAD_ZONE ? 0.0f : 1 * Mathf.Sign(dashInput.x);
        dashInput.y = Mathf.Abs(dashInput.y) < DEAD_ZONE ? 0.0f : 1 * Mathf.Sign(dashInput.y);
    }

    private void FixedUpdate()
    {
        Run();
        if (lastJumpPressedTime > 0.1f && CanJump())
        {
            Jump();
        }

        if (lastDashPressedTime > 0.1f && CanDash())
        {
            isDashing = true;
            Dash();
        }
    }

    private void SwitchPlayerDirection(bool facingTowardsRight)
    {
        if (facingTowardsRight == IsFacingRight)
            return;


        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        IsFacingRight = !IsFacingRight;
    }

    private void Run()
    {
        if (isDashing)
            return;

        //calculate the direction we want to run to
        float targetSpeed = moveInput.x * controllerData.maxRunSpeed;

        #region Calculate AccelRate
        float accelRate;
        if (lastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controllerData.runAccelerationAmount : controllerData.runDeccelerationAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controllerData.runAccelerationAmount * controllerData.accelerationInAir : controllerData.runDeccelerationAmount * controllerData.deccelerationInAir;

        #endregion

        if (controllerData.softLanding && CharacterRigidBody.velocity.y < controllerData.jumpLandTimeThreshold)
        {

            accelRate *= controllerData.jumpLandAccelerationMultiplier;
            targetSpeed *= controllerData.jumpLandMaxSpeedMultiplier;
        }
        else if ((isJumping || isJumpFalling) && CharacterRigidBody.velocity.y < controllerData.jumpHangTimeThreshold)
        {

            accelRate *= controllerData.jumpHangAccelerationMultiplier;
            targetSpeed *= controllerData.jumpHangMaxSpeedMultiplier;
        }

        //different between desired and current velocity
        float speedDiff = targetSpeed - CharacterRigidBody.velocity.x;
        //applied a scaled force to move the player
        CharacterRigidBody.AddForce(speedDiff * accelRate * Vector2.right, ForceMode2D.Force);
        /*
         * RigidBody.velocity = new Vector2(RRigidBodyB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
         */

    }

    private void Jump()
    {
        Debug.Log("Jumping");
        isJumpPressed = false;
        isJumping = true;
        isJumpFalling = false;
        isJumpCut = false;
        if (isDashing)
        {
            Debug.Log("isDashing");
            Vector2 dashForce = moveInput;
            if (dashForce.magnitude == 0)
            {
                dashForce = IsFacingRight ? Vector2.right : Vector2.left;
            }
            dashForce.x *= controllerData.dashForceX;
            dashForce.y *= controllerData.dashForceY;
            CharacterRigidBody.AddForce(dashForce * moveInput, ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log("OnJump");
            CharacterRigidBody.AddForce(controllerData.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

    }

    private void Dash()
    {
        Vector2 dashForce = dashInput;
        if (dashInput.magnitude == 0)
        {
            dashForce = IsFacingRight ? Vector2.right : Vector2.left;
            //when dash is single sided
            dashForce.x *= controllerData.dashForce;
            dashForce.y *= controllerData.dashForce;
        }
        else if (dashInput.magnitude == 1)
        {
            //when dash is single sided
            dashForce.x *= controllerData.dashForce;
            dashForce.y *= controllerData.dashForce;
        }
        else
        {
            //when diagonal dash we have more upward force
            dashForce.x *= controllerData.dashForceX;
            dashForce.y *= controllerData.dashForceY;
        }

        gravityScale = EGravityScale.EDASH;
        SetGravityScale(controllerData.gravityScale * controllerData.dashGravityMultiplier);
        CharacterRigidBody.velocity = new Vector2(0, 0);
        CharacterRigidBody.AddForce(dashForce, ForceMode2D.Impulse);
        dashWaitTime = controllerData.dashTimeToApex;

        isDashEnabled = false;
        isJumping = false;
        isJumpFalling = false;
        isJumpCut = false;

        DashPressed();
    }

    void CheckIfShouldDisableDash()
    {
        if (isDashing)
        {
            if (dashWaitTime < 0.0f)
            {
                if (CheckIfGrounded())
                {
                    isDashEnabled = true;
                    isDashing = false;
                    gravityScale = EGravityScale.ENORMAL;
                    SetGravityScale(controllerData.gravityScale);
                }
                else
                {
                    isDashing = false;
                }
            }
            dashWaitTime -= Time.deltaTime;
        }
    }

    private bool CanJump()
    {
        return !isJumping && lastOnGroundTime > 0.0f;
    }

    private bool CanDash()
    {
        return !isDashing && isDashEnabled;
    }

    private bool CheckIfGrounded()
    {
        Debug.Log("CheckIfGrounded");
        Vector3 lineStart = transform.position + Vector3.down * hitOffset;
        Vector3 lineEnd = lineStart + Vector3.down * hitDistance;
        RaycastHit2D hit = Physics2D.Raycast(lineStart, Vector2.down, hitDistance);
    
        if (hit.collider != null) // && hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Debug.Log(hit.collider.gameObject.name);
            return true;
        }
        return false;
    }


    private void SetGravityScale(float scale)
    {
        CharacterRigidBody.gravityScale = scale;
    }
}