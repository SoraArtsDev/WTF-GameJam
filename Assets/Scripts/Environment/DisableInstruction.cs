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
    /// You may delete all of the stuff inside here. 
    /// Just remember to stick to the formating
    public class DisableInstruction : MonoBehaviour
    {
        [SerializeField] private float durationToDisable;
        private bool visited;

        private void OnEnable()
        {
            if(!visited)
                StartCoroutine(DisableObject());
        }

        private IEnumerator DisableObject()
        {
            yield return new WaitForSecondsRealtime(durationToDisable);
            visited = true;
            gameObject.SetActive(false);
        }
    }
}