// Developed by Sora
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sora.Environment
{
    public class LiftSwitch : MonoBehaviour
    {
        [SerializeField] private Transform lift;
        [SerializeField] private WayPoint[] moveBetween;
        [SerializeField] private float timeToReach;

        private CharacterController2D player;
        private bool playerAroundSwitch;
        private bool switchCD;
        private int wpIndex;
        private Vector3 initialPos;
        private float currentTime;

        private void Start()
        {
            player = FindObjectOfType<CharacterController2D>();
            player.inputMap.PlayerController.Interact.started += OnInteraction;

            initialPos = moveBetween[0].position;
            currentTime = 0.0f;
            wpIndex = 1;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                playerAroundSwitch = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                playerAroundSwitch = false;
            }
        }

        void OnInteraction(InputAction.CallbackContext context)
        {
            if(playerAroundSwitch && !switchCD)
            {
                switchCD = true;

                StartCoroutine(MoveLift());
            }
        }

        private IEnumerator MoveLift()
        {
            while(currentTime <= timeToReach)
            {
                lift.transform.localPosition = Vector3.Lerp(initialPos, moveBetween[wpIndex].position, currentTime / timeToReach);

                currentTime += Time.deltaTime;

                yield return null;
            }

            lift.transform.localPosition = moveBetween[wpIndex].position;
            currentTime = 0.0f;
            initialPos = moveBetween[wpIndex].position;
            wpIndex++;
            if (wpIndex >= moveBetween.Length)
                wpIndex = 0;

            switchCD = false;
        }

    }
}