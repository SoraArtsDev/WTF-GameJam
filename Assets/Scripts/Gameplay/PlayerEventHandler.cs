// Developed by Sora
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
        [SerializeField] private SpriteRenderer vent;
        [SerializeField] private GameObject possessDialogues;

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

        public void SetupVent()
        {
            if (vent)
            {
                vent.color = new Color(vent.color.r, vent.color.g, vent.color.b, 0.5f);
                vent.GetComponent<Collider2D>().enabled = false;
                possessDialogues.SetActive(true);
            }
        }
    }
}