using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player States
public enum EPlayerState
{
    ENONE,
    EIDLE,
    EINACTION,
    ERUNNING,
    EGLITCHING,
}

// Player States
public enum EPlayerHealthState
{
    ELOW,
    EFULL

    // additional status effect like bleeding or damaging
    // effects states can be added here
}

// Player States
public enum EPlayerUnlockablePowerState
{
    ENONE,
    EDASH,
    ETIMEDILATION

    // additional player unlockables like throwing grenades,...etc
    // unlockables can be added here
}

public class PlayerBase : MonoBehaviour
{
    private EPlayerHealthState myPlayerHealthState;
    private EPlayerState mPreviousPlayerState;
    private Animator mAnimator;
    private TrailRenderer trailRenderer;
    private bool hasTrail;
    private float dashTimer;
    // Property for current Player State
    public EPlayerState PlayerState { get; set; }

    // Property for current Player Health State
    EPlayerHealthState PlayerHealthState
    {
        get { return myPlayerHealthState; }
        set { myPlayerHealthState = value; }
    }

    private CharacterController2D characterController;
    // TODO_OP: Uncomment these when you set up the states
    // Property for current Player unlockable power State
    //private EPlayerUnlockablePowerState myPlayerUnlockablePowerState;
    //EPlayerUnlockablePowerState PlayerUnlockablePowerState
    //{
    //    get { return myPlayerUnlockablePowerState; }
    //    // don't set the player's power unlockable state since this is only for references
    //    // maintain a separate var which keeps track of unlockables player currently has achieved.
    //}

    // Start is called before the first frame update
    private void Start()
    {
        PlayerState = EPlayerState.ENONE;
        mPreviousPlayerState = EPlayerState.ENONE;
        myPlayerHealthState = EPlayerHealthState.EFULL;

        mAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        characterController.DashPressed += DashPressed;
        dashTimer = 0;
        hasTrail = false;

        trailRenderer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
      //  mAnimator.SetFloat("Speed", Mathf.Abs(characterController.CharacterRigidBody.velocity.x));
        
        //if(hasTrail)
        //{
        //    dashTimer -= Time.deltaTime;
        //    if(dashTimer<0)
        //    {
        //        trailRenderer.enabled = false;
        //        hasTrail = false;
        //    }
        //}
    }

    void DashPressed()
    {
       // dashTimer = .2f;
       // hasTrail = true;
       // trailRenderer.enabled = true;       
    }
}
