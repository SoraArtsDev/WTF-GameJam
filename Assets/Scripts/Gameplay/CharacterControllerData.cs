using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterControllerData")]
public class CharacterControllerData : ScriptableObject
{

    [Header("Gravity")]
   // [HideInInspector]
    public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    //[HideInInspector]
    public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    [Space(20)]

    [Header("Run")]
    public float maxRunSpeed; //Target speed we want the player to reach.
    public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
    [HideInInspector]
    public float runAccelerationAmount; //The actual force (multiplied with speedDiff) applied to the player.
    [HideInInspector]
    public float runDeccelerationAmount; //Actual force (multiplied with speedDiff) applied to the player .

    public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    [Space(5)]
    [Range(0f, 1)] public float accelerationInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0f, 1)] public float deccelerationInAir;

    [Header("Jump")]
    public float jumpHeight; //Height of the player's jump
    public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    public float jumpCutGravityMultiplier; //Multiplier to increase gravity if the player releases the jump button while still jumping
    [Range(0f, 1)]
    public float jumpHangGravityMultiplier; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex
    [Space(0.5f)]
    public float jumpHangAccelerationMultiplier;
    public float jumpHangMaxSpeedMultiplier;


    [Header("Hop")]
    public float hopHeight; //Height of the player's jump
    public float hopTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    public float hopCutGravityMultiplier; //Multiplier to increase gravity if the player releases the jump button while still jumping
    public float nextHopWaitTime; //wait before next Hop
    public float hopDistanceX; 
    public float hopDistanceY; 


    [Space(0.5f)]
    [Range(0f, 1)]
    public float jumpLandGravityMultiplier; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpLandTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex
    public float jumpLandAccelerationMultiplier;
    public float jumpLandMaxSpeedMultiplier;

    [HideInInspector]
    public float jumpForce; //The actual force applied (upwards) to the player when they jump.
    public float hopForceX; //The actual force applied (upwards) to the player when they jump.
    public float hopForceY; //The actual force applied (upwards) to the player when they jump.


    [Header("Dash")]
    [Space(0.5f)]
    public float dashDistanceX; //Distance of the player's dashX
    public float dashDistanceY; //Distance of the player's dashY
    public float dashDistance; //Distance of the player's dashY
    public float dashTimeToApex; //Time between applying the dash force and reaching the desired distance.
    public float dashGravityMultiplier; //Time between applying the dash force and reaching the desired distance.

    [HideInInspector]
    public float dashForceX; //The actual force applied sidewards to the player when they dash.
    [HideInInspector]
    public float dashForceY; //The actual force applied upwards to the player when they dash.
    [HideInInspector]
    public float dashForce; //horizontal force.


    [Header("Assists")]
    public float coyoteTime; //this is bufferedTime used for Jumps.
    public float jumpPressBufferTime; //this is bufferedTime used for jump input.
    public float dashPressedBufferTime; //this is bufferedTime used for dash input.
    public bool softLanding; //this is bufferedTime used for Jumps nad inputs.


    [Header("SouldSpeed")]
    public float speedXForSoul;
    public float speedYForSoul;
    public float soulGravityMultiplier; //Multiplier to increase gravity if the player releases the jump button while still jumping


    [Header("Detachables")]
    public float torsoThrowForce; 
    public float torsoThrowTorque; 

    [Header("Draggable")]
    public float speedForDrag;
    public float dragSpeedPullMultiplier;
    public float dragSpeedPushMultiplier;

    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelerationAmount = (1/Time.fixedDeltaTime * runAcceleration) / maxRunSpeed;
        runDeccelerationAmount = (1 / Time.fixedDeltaTime * runDecceleration) / maxRunSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;


        float strength = (2 * hopDistanceX) / (hopTimeToApex * hopTimeToApex);
        hopForceX = Mathf.Abs(strength) * hopTimeToApex;
        strength = -(2 * hopDistanceY) / (hopTimeToApex * hopTimeToApex);
        hopForceY = Mathf.Abs(strength) * hopTimeToApex;
        //hopForceY = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        /*
        * f = ma
        * d = v0 + 1/2 *a*t^2 // assuming v0  = 0
        * a = 2*d/t^2
        * f = 2*d/t^2
        */
        float strengthX = (2 * dashDistanceX) / (dashTimeToApex* dashTimeToApex);
        dashForceX = Mathf.Abs(strengthX) * dashTimeToApex;

        float strengthY = (2 * dashDistanceY) / (dashTimeToApex * dashTimeToApex);
        dashForceY = Mathf.Abs(strengthY) * dashTimeToApex;

        strength = (2 * dashDistance) / (dashTimeToApex * dashTimeToApex);
        dashForce = Mathf.Abs(strength) * dashTimeToApex;
        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxRunSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxRunSpeed);
        #endregion
    }
}
