using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : MonoBehaviour
{
    public Animator fireAnimator;
    public Animator myAnimator;
    public float waitDuration = 2.0f;
    public float elapsedTime = 0.0f;
    public bool hasWaitTime;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        fireAnimator.SetBool("isIdle", false);
        myAnimator.SetBool("isIdle", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasWaitTime)
        {
            if (elapsedTime >= waitDuration)
            {
                SetDefferedState();
            }
            elapsedTime += Time.deltaTime;
        }
    }

    public void FinishedFiring()
    {
        myAnimator.SetBool("isIdle", true);
        fireAnimator.SetBool("isIdle", true);
        hasWaitTime = true;
        elapsedTime = 0.0f;
    }

    void SetDefferedState()
    {
        hasWaitTime = false;
        myAnimator.SetBool("isIdle", false);
    }

    public void Fire()
    {
        fireAnimator.SetBool("isIdle", false);
    }
}
