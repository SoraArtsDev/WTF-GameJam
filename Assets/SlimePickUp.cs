using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePickUp : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO:
        // increase slime counter
        //Debug.Log("slime pickup");
        Destroy(gameObject);


    }
}
