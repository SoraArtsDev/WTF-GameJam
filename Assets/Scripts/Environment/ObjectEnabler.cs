// Developed by Pluto
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora 
{
    /// You may delete all of the stuff inside here. 
    /// Just remember to stick to the formating
    public class ObjectEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject objectToEnable;
        [SerializeField] private bool fireEvent;

        [ShowIf("fireEvent", true)]
        [SerializeField] Events.SoraEvent onCollision;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (objectToEnable)
                {
                    objectToEnable.SetActive(true);
                }
                onCollision.InvokeEvent();
            }
        }
    }
}