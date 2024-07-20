using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickables : MonoBehaviour
{
    bool isTouchingPlayer;
    public float interactableTouchCheckRadius;
    public CharacterController2D.EPickableType pickableType;
    CharacterController2D controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = transform.position - controller.transform.position;
        isTouchingPlayer = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
        controller.SetPickable(isTouchingPlayer ? transform : null);
#if UNITY_EDITOR
        distance = transform.position - controller.transform.position;
        bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
        DebugDraw.DrawWireCircle(transform.position, transform.rotation, interactableTouchCheckRadius, isTouching ? Color.red : Color.white);
#endif
    }
}
