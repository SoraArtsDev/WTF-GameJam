using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggables : MonoBehaviour
{
    bool isTouchingPlayer;
    CharacterController2D controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Touching Player");
            //controller.SetDraggable();
        }
    }
}
