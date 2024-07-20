using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePickUp : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(Sora.Managers.GameManager.instance!=null)
        {
            Sora.Managers.GameManager.instance.IncrementSlimeCount();
        }
        //Debug.Log("slime pickup");
        Destroy(gameObject);


    }
}
