using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickables : MonoBehaviour
{
    bool isTouchingPlayer;
    public float interactableTouchCheckRadius;
    public CharacterController2D.EPickableType pickableType;
    CharacterController2D controller;
    SphereCollider coll;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // = transform.position - controller.transform.position;
        //isTouchingPlayer = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;

#if UNITY_EDITOR
        Vector3 distance = transform.position - controller.transform.position;
        bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
        DebugDraw.DrawWireCircle(transform.position, transform.rotation, interactableTouchCheckRadius, isTouching ? Color.red : Color.white);
#endif
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            controller.SetPickable(transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (controller.GetPickable() == this)
                controller.SetPickable(null);
        }
    }

    public void Consume()
    {
        switch (pickableType)
        {
            case CharacterController2D.EPickableType.ESLIME:
                {
                    if (Sora.Managers.GameManager.instance != null)
                    {
                        Sora.Managers.GameManager.instance.IncrementSlimeCount();
                    }
                }
                break;
            case CharacterController2D.EPickableType.EHEAD:
                {
                    controller.SetPlayerState(CharacterController2D.EPlayerState.EHEAD);
                }
                break;
            case CharacterController2D.EPickableType.ELEGS:
                {
                    controller.SetPlayerState(CharacterController2D.EPlayerState.ELEGS);
                }
                break;
            case CharacterController2D.EPickableType.EHANDS:
                {
                    controller.SetPlayerState(CharacterController2D.EPlayerState.E_FULL_BODY);
                }
                break;
            case CharacterController2D.EPickableType.ETORSO:
                {
                    controller.SetPlayerState(CharacterController2D.EPlayerState.ETORSO);
                }
                break;
            default:break;
        }

        GameObject.Destroy(gameObject);

    }
}
