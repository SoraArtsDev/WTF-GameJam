// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sora.InputSystem
{
    /// <summary>
    /// A container scriptable object that holds the Unity Generated InputSystem Wrapper's instance
    /// Only one of these should exist in the project
    /// </summary>
    [CreateAssetMenu(fileName = "BindingsContainer", menuName = "Sora /Input System /PlayerBindings Container")]
    [System.Serializable]
    public class BindingsContainer : ScriptableObject
    {
        public PCKeyBindings keyBindings { private set; get; }
        public GameplayInputReader gameplayInputReader;

        private void OnEnable()
        {
            if (this.keyBindings == null)
            {
                keyBindings = new PCKeyBindings();
            }

            gameplayInputReader.Enable();
        }
    }
}
