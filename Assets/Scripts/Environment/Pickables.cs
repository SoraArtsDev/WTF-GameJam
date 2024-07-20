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
                break;
            case CharacterController2D.EPickableType.ELEGS:
                break;
            case CharacterController2D.EPickableType.EHANDS:
                break;
            case CharacterController2D.EPickableType.ETORSO:
                break;
            default:break;
        }

        GameObject.Destroy(gameObject);

    }
}
