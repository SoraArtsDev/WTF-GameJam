using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{

    private const float DEAD_ZONE = 0.27f;

    public enum EPlayerState
    {
        ENONE,
        EHEAD,
        ETORSO,
        EHANDS,
        E_FULL_BODY,
    };

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
    public float hitOffsetHead;
    public float originalOffset;


    public EGravityScale gravityScale;
    public EPlayerState  playerState;

    private float lastOnGroundTime;
    private float lastJumpPressedTime;
    private float lastDashPressedTime;
    private float dashWaitTime;
    private float hopWaitTime;

    //Components
    public Rigidbody2D CharacterRigidBody { get; private set; }

    //Jump
    private bool isJumping;
    private bool isJumpCut;
    private bool isDashing;
    private bool isJumpFalling;
    private bool isJumpReleased;
    private bool isDashPressed;
    private bool isDashEnabled;
    private bool isRunning;
    private bool isIdle;
    private bool isTorsoRemoved;
    private bool isTouchingDetachedTorso;

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


    private Vector2 colliderSize;



    //Detachables
    private SpriteRenderer spriteRenderer;
    public Sprite sprHead;
    public Sprite sprFullbody;
    public GameObject torsoPrefab;
    public GameObject instantiatedTorso;
    public float detachedTorsoCheckRadius;

    // Start is called before the first frame update
    void Awake()
    {
        CharacterRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inputMap = new InputMap();
        inputMap.PlayerController.Enable();
        inputMap.PlayerController.Jump.started += OnJump;
        //inputMap.PlayerController.Dash.started += OnDash;
        inputMap.PlayerController.RemoveTorso.started += OnRemoveTorso;

        IsFacingRight = true;
        isJumping = false;
        isDashing = false;
        isTorsoRemoved = false;
        isTouchingDetachedTorso = false;
        isDashPressed = false;
        isJumpReleased = false;
        isDashEnabled = true;

        isRunning = false; ;
        isIdle = true;

        gravityScale = EGravityScale.ENORMAL;
        playerState = EPlayerState.ETORSO;
        lastOnGroundTime = 0.0f;
        lastJumpPressedTime = 0.0f;

        colliderSize = GetComponent<BoxCollider2D>().size;
    }


    void OnJump(InputAction.CallbackContext context)
    {
        lastJumpPressedTime = controllerData.jumpPressBufferTime;
    }

    void OnDash(InputAction.CallbackContext context)
    {
        isDashPressed = true;
        lastDashPressedTime = controllerData.dashPressedBufferTime;
    }


    void OnRemoveTorso(InputAction.CallbackContext context)
    {

        if(isTorsoRemoved)
        {

            Vector3 distance = transform.position - instantiatedTorso.transform.position;
            bool isTouching = distance.sqrMagnitude < detachedTorsoCheckRadius * detachedTorsoCheckRadius;
            if(!isTouching)
                return;
        }

        isTorsoRemoved = !isTorsoRemoved;
        if (isTorsoRemoved)
        {
            GetComponent<BoxCollider2D>().size = new Vector2(colliderSize.x, colliderSize.y / 2);
            hitOffset = hitOffsetHead;
            spriteRenderer.sprite = sprHead;
            playerState = EPlayerState.EHEAD;

        }
        else
        {
            GetComponent<BoxCollider2D>().size = new Vector2(colliderSize.x, colliderSize.y);
            hitOffset = originalOffset;
            spriteRenderer.sprite = sprFullbody;
            playerState = EPlayerState.ETORSO;
        }
        ConfigureTorso();
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

        if (isTorsoRemoved)
        {
            //DrawCircle(new Vector2(instantiatedTorso.transform.position.x, instantiatedTorso.transform.position.y), detachedTorsoCheckRadius, 36);
            Vector3 distance = transform.position - instantiatedTorso.transform.position;
            bool isTouching = distance.sqrMagnitude < detachedTorsoCheckRadius * detachedTorsoCheckRadius;

            DrawWireCircle(instantiatedTorso.transform.position, instantiatedTorso.transform.rotation, detachedTorsoCheckRadius, isTouching? Color.green : Color.white);
        }

#endif

        if (isTorsoRemoved)
        {
            var rb = instantiatedTorso.GetComponent<Rigidbody2D>();
            if (Mathf.Approximately(rb.velocity.y , 0.0f))
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            } 
        }

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
                hopWaitTime -= Time.deltaTime;
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
        if (playerState == EPlayerState.EHEAD)
            Run();
        else if (playerState == EPlayerState.ETORSO && CanHop())
        {
            bool canApply =  (moveInput.x * controllerData.maxRunSpeed) !=0;
            if (canApply)
            {
                Hop();
                hopWaitTime = controllerData.nextHopWaitTime;
            }
        }

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

    //imitating Jump as Hop
    private void Hop()
    {
        {
            isJumping = true;
            isJumpFalling = false;
            isJumpCut = false;
            float xforce = moveInput.x * controllerData.hopForceX;
            float yforce = Mathf.Abs(moveInput.x * controllerData.hopForceY);
            lastJumpPressedTime = controllerData.jumpPressBufferTime;
            CharacterRigidBody.AddForce(xforce * Vector2.right, ForceMode2D.Impulse);
            CharacterRigidBody.AddForce(yforce * Vector2.up, ForceMode2D.Impulse);
        }
    }

    private void Jump()
    {
        Debug.Log("Jumping");
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
        return !isJumping && lastOnGroundTime > 0.0f && playerState != EPlayerState.EHEAD && playerState != EPlayerState.ETORSO;
    }

    private bool CanHop()
    {
        return !isJumping && lastOnGroundTime > 0.0f && hopWaitTime < 0.0f && playerState == EPlayerState.ETORSO;
    }

    private bool CanDash()
    {
        return !isDashing && isDashEnabled && playerState == EPlayerState.ENONE;
    }


    private void ConfigureTorso()
    {
        if (isTorsoRemoved)
        {
            Vector2 direction = new Vector2(moveInput.x * 0.1f , 1);
            Vector3 spawnPoint = new Vector3(direction.x, 0, 0);
            spawnPoint += transform.position;
            direction.Normalize();
            instantiatedTorso = GameObject.Instantiate(torsoPrefab, spawnPoint, Quaternion.identity);
            float force = Random.Range(1.5f,controllerData.torsoThrowForce);
            float torque = Random.Range(.1f,controllerData.torsoThrowTorque);
            instantiatedTorso.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            instantiatedTorso.GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Impulse);
        }
        else
        {
            GameObject.Destroy(instantiatedTorso);
        }
    }

    private bool CheckIfGrounded()
    {
        Vector3 lineStart = transform.position + Vector3.down * hitOffset;
        Vector3 lineEnd = lineStart + Vector3.down * hitDistance;
        RaycastHit2D hit = Physics2D.Raycast(lineStart, Vector2.down, hitDistance);
    
        if (hit.collider != null  && hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            return true;
        }
        return false;
    }

    public const float TAU = 6.283185307179586f;
    public static Vector2 GetUnitVectorByAngle(float angleRad)
    {
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void DrawCircle(Vector2 center, float radius, int segments)
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector2 start = center + new Vector2(x, y);

            angle += 360f / segments;

            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector2 end = center + new Vector2(x, y);

            Debug.DrawLine(start, end, Color.red, 1f);
        }
    }

    public static void DrawWireCircle(Vector3 pos, Quaternion rot, float radius, Color color, int detail = 32)
    {
        Vector3[] points3D = new Vector3[detail];
        for (int i = 0; i < detail; ++i)
        {
            float t = (float)(i) / detail;
            float angRad = TAU * t;
            Vector2 points2D = GetUnitVectorByAngle(angRad);
            points2D *= radius;
            points3D[i] = pos + rot * points2D;
        }

        for (int i = 0; i < detail - 1; ++i)
        {
            Debug.DrawLine(points3D[i], points3D[i + 1],color);
        }
        Debug.DrawLine(points3D[detail - 1], points3D[0],color);
    }

    private void SetGravityScale(float scale)
    {
        CharacterRigidBody.gravityScale = scale;
    }
}