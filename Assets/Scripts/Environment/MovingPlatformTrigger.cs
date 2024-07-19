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
    public class MovingPlatformTrigger : MonoBehaviour
    {
        [SerializeField] private MovingPlatform pairedPlaform;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            pairedPlaform.StartMoving();
        }
    }
}