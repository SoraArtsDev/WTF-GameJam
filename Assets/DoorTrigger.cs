using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject door;

    public bool isOpened = false;
    float duration = 0.0f;
    float elapsedTime = 0.0f;
    static Vector3 finalPos = new Vector3(0, 4, 0);
    static Vector3 startPos;

    private void Start()
    {
        Reset();
    }

    private void Awake()
    {
        Reset();
    }

    private void Reset()
    {
        startPos = door.transform.position;
        finalPos = startPos + new Vector3(0, 4, 0);
        duration = 2.0f;
        elapsedTime = 0.0f;
    }

    private void Update()
    {
        if (isOpened)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            door.transform.position = Vector3.Lerp(startPos, finalPos, t);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpened)
        {
            if (Sora.Managers.GameManager.instance != null)
            {
                if (Sora.Managers.GameManager.instance.HaveCollectedRequiredSlime() == false)
                {
                    return;
                }
            }
            isOpened = true;
        }

    }

}
