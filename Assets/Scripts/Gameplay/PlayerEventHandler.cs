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
    public class PlayerEventHandler : MonoBehaviour
    {
        private CharacterController2D characterController;

        private void OnEnable()
        {
            characterController = GetComponent<CharacterController2D>();
        }

        public void DisableSoulMovement(Component invoker, object data)
        {
            characterController.SetSoulMult(0, 0);
        }

        public void EnableADDirections(Component invoker, object data)
        {
            characterController.SetSoulMult(1, 0);
        }

        public void EnableWSDirections(Component invoker, object data)
        {
            characterController.SetSoulMult(1, 1);
        }

    }
}