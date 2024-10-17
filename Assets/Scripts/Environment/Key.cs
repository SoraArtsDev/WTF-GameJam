// Developed by Sora
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Environment
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private Lock pairedLock;
        [SerializeField] private Animator anim;

        private void OnTriggerStay2D(Collider2D collision)
        {
            pairedLock.Unlock(true);
            
            if(anim)
                anim.Play("Unlock", 0);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            pairedLock.Unlock(false);
            
            if (anim)
                anim.Play("Unlock", 0);
        }
    }
}