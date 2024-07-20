using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public Rigidbody2D body;

    public float leftPushRange;
    public float rightPushRange;
    public float velocityThreshold;

    private void Start()
    {
        body=GetComponent<Rigidbody2D>();
        body.angularVelocity = velocityThreshold;
    }
    void Update()
    {
        if(transform.rotation.z > 0 
            && transform.rotation.z < rightPushRange
            && body.angularVelocity > 0
            && body.angularVelocity < velocityThreshold )
        {
            body.angularVelocity= velocityThreshold;
        }
        else if(transform.rotation.z < 0
            && transform.rotation.z > leftPushRange
            && body.angularVelocity < 0
            && body.angularVelocity > velocityThreshold * -1)
        {
            body.angularVelocity = velocityThreshold * -1;
        }
    }
}
