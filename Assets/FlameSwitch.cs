using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSwitch : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    { 

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("StopTrigger");
    }
}
