// Developed by Pluto
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
    public class Lock : MonoBehaviour
    {
        [SerializeField] private Key pairedKey;
        [SerializeField] private Animator anim;
        private Collider2D collider;

        private void OnEnable()
        {
            collider = GetComponent<Collider2D>();
        }

        public void Unlock(bool unlock)
        {
            collider.enabled = !unlock;

            if (unlock && anim)
                anim.Play("Unlock", 0);
            else
                anim.Play("Lock", 0);
        }        
    }
}