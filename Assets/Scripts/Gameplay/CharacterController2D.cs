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
        ESOUL,
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
    public EPlayerState  lastState;

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
    private bool isDraggingObject;
    private bool isThrowingObject;

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
    private Vector2 soulMult;
    private Vector3 prevPosition;



    //Detachables
    public EPlayerState startingState;
    private SpriteRenderer spriteRenderer;
    public Sprite sprHead;
    public Sprite sprFullbody;
    public GameObject torsoPrefab;
    public GameObject handPrefab;
    private GameObject instantiatedTorso;
    public float interactableTouchCheckRadius;


    //Interactables
    private Transform draggable;
    private float dragSpeedMultiplier;

    // Start is called before the first frame update
    void Awake()
    {
        CharacterRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        EnsureInput();

        IsFacingRight = true;
        isJumping = false;
        isDashing = false;
        isTorsoRemoved = false;
        isTouchingDetachedTorso = false;
        isDashPressed = false;
        isJumpReleased = false; 
        isDashEnabled = true;
        isDraggingObject = false;
        isThrowingObject = false;

        isRunning = false; ;
        isIdle = true;

        gravityScale = EGravityScale.ENORMAL;
        lastState = playerState;

        lastOnGroundTime = 0.0f;
        lastJumpPressedTime = 0.0f;
        soulMult = Vector2.zero;
        dragSpeedMultiplier = 1.0f;

        colliderSize = GetComponent<BoxCollider2D>().size;
        SetPlayerState(startingState);
        isTorsoRemoved = true;
        var instantiatedTorso = GameObject.Find("torso");
        if (instantiatedTorso)
        {
            var rb = instantiatedTorso.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }


    public void EnsureInput()
    {
        if (inputMap == null)
        {
            inputMap = new InputMap();
            inputMap.PlayerController.Enable();
            inputMap.PlayerController.Jump.started += OnJump;
            //inputMap.PlayerController.Dash.started += OnDash;
            inputMap.PlayerController.RemoveTorso.started += OnRemoveTorso;
            inputMap.PlayerController.MoveObject.canceled += OnMoveDone;
            inputMap.PlayerController.MoveObject.started += OnMoveObject;
            inputMap.PlayerController.ThrowObject.started += OnThrowObject;

        }
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
        if(playerState == EPlayerState.EHEAD)
        {
            Vector3 distance = transform.position - instantiatedTorso.transform.position;
            bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
            if(!isTouching)
                return;
        }

        isTorsoRemoved = !isTorsoRemoved;
        if (isTorsoRemoved && playerState == EPlayerState.ETORSO)
        {
            Vector2 direction = new Vector2(moveInput.x * 0.1f, 1);
            Vector3 spawnPoint = new Vector3(direction.x, 0, 0);
            spawnPoint += transform.position;
            direction.Normalize();
            instantiatedTorso = GameObject.Instantiate(torsoPrefab, spawnPoint, Quaternion.identity);
            float force = Random.Range(1.5f, controllerData.torsoThrowForce);
            float torque = Random.Range(.1f, controllerData.torsoThrowTorque);
            instantiatedTorso.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            instantiatedTorso.GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Impulse);
        }
        ConfigureTorso(isTorsoRemoved);
    }

    void OnMoveObject(InputAction.CallbackContext context)
    {
        if (playerState != EPlayerState.E_FULL_BODY)
            return;

        if(draggable)
        {
            isDraggingObject = true;
            Debug.Log("OnMoveObject");
        }
    }
    
    void OnMoveDone(InputAction.CallbackContext context)
    {
        isDraggingObject = false;
        Debug.Log("OnMoveDone");
        draggable = null;
    }

    void OnThrowObject(InputAction.CallbackContext context)
    {
        isThrowingObject = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (lastState != playerState)
        {
            lastState = playerState;
        }

        if(playerState== EPlayerState.ESOUL)
        {
            moveInput = inputMap.PlayerController.Movement.ReadValue<Vector2>();

            Vector3 pos = transform.position;
            CharacterRigidBody.MovePosition(new Vector2(pos.x + soulMult.x*moveInput.x*controllerData.speedXForSoul * Time.deltaTime, pos.y + soulMult.y * moveInput.y * controllerData.speedXForSoul * Time.deltaTime));
            SetGravityScale(controllerData.soulGravityMultiplier);
            return;
        }

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

        if (instantiatedTorso)
        {
            //DrawCircle(new Vector2(instantiatedTorso.transform.position.x, instantiatedTorso.transform.position.y), interactableTouchCheckRadius, 36);
            Vector3 distance = transform.position - instantiatedTorso.transform.position;
            bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;

            DebugDraw.DrawWireCircle(instantiatedTorso.transform.position, instantiatedTorso.transform.rotation, interactableTouchCheckRadius, isTouching? Color.green : Color.white);
        }

        if (draggable)
        {
            Vector3 distance = transform.position - draggable.position;
            bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
            DebugDraw.DrawWireCircle(draggable.position, draggable.rotation, interactableTouchCheckRadius, isTouching ? Color.red : Color.white);
        }

#endif
        if (isTorsoRemoved && instantiatedTorso)
        {
            var rb = instantiatedTorso.GetComponent<Rigidbody2D>();
            if (Mathf.Approximately(rb.velocity.y , 0.0f))
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            } 
        }

        if(isThrowingObject)
        {
            Vector2 direction = new Vector2(moveInput.x * 0.1f, 1);
            Vector3 spawnPoint = new Vector3(direction.x, 0, 0);
            spawnPoint += transform.position;
            direction.Normalize();
            var hand = GameObject.Instantiate(handPrefab, spawnPoint, Quaternion.identity);
            float len = moveInput.sqrMagnitude;
            direction = moveInput;
            if (Mathf.Approximately(len, 0.0f))
            {
                direction = IsFacingRight ? Vector2.right : Vector2.left;
            }

            hand.GetComponent<Boomerang>().ThrowBoomerang(direction, CharacterRigidBody,controllerData);
            isThrowingObject = false;
        }

        //Check if Grounded
        if (!isJumping && !isDraggingObject)
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
        isIdle = !isDraggingObject && !isRunning && !isJumping && !isJumpFalling && !isJumpCut && lastOnGroundTime > 0.1f;
        #endregion
    }

    void CheckIfInDeadZone()
    {
        dashInput.x = Mathf.Abs(dashInput.x) < DEAD_ZONE ? 0.0f : 1 * Mathf.Sign(dashInput.x);
        dashInput.y = Mathf.Abs(dashInput.y) < DEAD_ZONE ? 0.0f : 1 * Mathf.Sign(dashInput.y);
    }

    private void FixedUpdate()
    {
        if (playerState == EPlayerState.EHEAD || playerState == EPlayerState.E_FULL_BODY)
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

        dragSpeedMultiplier = 1.0f;
        if (isDraggingObject && draggable)
        {
            Vector3 pos = transform.position;
            draggable.position = new Vector3(draggable.position.x + moveInput.x * controllerData.speedForDrag * Time.deltaTime, draggable.position.y, 0);

            bool isPush = (moveInput.x > 0 && draggable.position.x > transform.position.x) || (moveInput.x< 0 && draggable.position.x < transform.position.x);
            dragSpeedMultiplier = isPush ? controllerData.dragSpeedPushMultiplier : controllerData.dragSpeedPullMultiplier;
        }
            //calculate the direction we want to run to
        float targetSpeed = moveInput.x * controllerData.maxRunSpeed;

        #region Calculate AccelRate
        float accelRate;
        if (lastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controllerData.runAccelerationAmount : controllerData.runDeccelerationAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controllerData.runAccelerationAmount * controllerData.accelerationInAir : controllerData.runDeccelerationAmount * controllerData.deccelerationInAir;

        #endregion

        accelRate *= dragSpeedMultiplier;
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
        return !isDraggingObject && !isJumping && lastOnGroundTime > 0.0f && playerState != EPlayerState.EHEAD && playerState != EPlayerState.ETORSO;
    }

    private bool CanHop()
    {
        return !isDraggingObject && !isJumping && lastOnGroundTime > 0.0f && hopWaitTime < 0.0f && playerState == EPlayerState.ETORSO;
    }

    private bool CanDash()
    {
        return !isDashing && isDashEnabled && playerState == EPlayerState.ENONE;
    }


    private void ConfigureTorso(bool isTorsoRemoved)
    {
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

            if (instantiatedTorso)
            {
                GameObject.Destroy(instantiatedTorso);
            }
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

    private void SetGravityScale(float scale)
    {
        CharacterRigidBody.gravityScale = scale;
    }


    public void SetSoulMult(int multX=0, int multY = 0)
    {
        soulMult = new Vector2(multX, multX);
    }

    public void SetPlayerState(EPlayerState state)
    {
        playerState = state;
        if(playerState == EPlayerState.EHEAD)
        {
            ConfigureTorso(true);
        } else if(playerState == EPlayerState.ETORSO)
        {
            ConfigureTorso(false);
        }
    }

    public void SetDraggable(Transform draggableObj) {
        draggable = draggableObj;
    }
}