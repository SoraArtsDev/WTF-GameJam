using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject door;

    bool isOpened = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isOpened)
        {
            if(Sora.Managers.GameManager.instance != null)
            {
                if(Sora.Managers.GameManager.instance.HaveCollectedRequiredSlime() == false)
                {
                    return;
                }
            }
            door.transform.position += new Vector3(0, 4, 0);
            isOpened = true;
        }
        
    }
   
}
