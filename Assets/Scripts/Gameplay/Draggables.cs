using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggables : MonoBehaviour
{
    bool isTouchingPlayer;
    public float interactableTouchCheckRadius;
    CharacterController2D controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = transform.parent.position - controller.transform.position;
        isTouchingPlayer = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
        controller.SetDraggable(isTouchingPlayer ? transform.parent : null);

#if UNITY_EDITOR
        distance = transform.parent.position - controller.transform.position;
        bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
        DebugDraw.DrawWireCircle(transform.position, transform.rotation, interactableTouchCheckRadius, isTouching ? Color.red : Color.white);
#endif
    }
}
