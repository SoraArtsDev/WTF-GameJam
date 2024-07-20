using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMovement : MonoBehaviour
{
    public float maxHeight = 5f;
    public float riseSpeed = 1f;
    public float fallSpeed = 5f;
    
    public float delayBetweenCycles = 1f;

    private bool isRising = true;
    private float delayTimer = 0f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (isRising)
        {
            // Slowly rise
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            // Check if reached max height
            if (transform.position.y >= maxHeight)
            {
                isRising = false;
            }
        }
        else
        {
            // Rapidly fall
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // Check if reached bottom position
            if (transform.position.y <= initialPosition.y)
            {
                isRising = true;
                // Ensure the spike is exactly at the bottom position
                transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
                // Start the delay timer
                delayTimer = delayBetweenCycles;
            }
        }
    }
}
