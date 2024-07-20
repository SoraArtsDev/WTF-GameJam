using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;         // Speed at which the platform moves
    public float topPosition = 5f;       // The highest point the platform will move to
    private float bottomPosition;    // The lowest point the platform will move to

    private bool movingUp = true;        // Direction flag

    private void Start()
    {
        bottomPosition = transform.position.y;
    }
    void Update()
    {
        if (movingUp)
        {
            // Move the platform up
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // Check if the platform has reached the top position
            if (transform.position.y >= topPosition)
            {
                movingUp = false;  // Start moving down
            }
        }
        else
        {
            // Move the platform down
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime;

            // Check if the platform has reached the bottom position
            if (transform.position.y <= bottomPosition)
            {
                movingUp = true;   // Start moving up
            }
        }
    }
}
