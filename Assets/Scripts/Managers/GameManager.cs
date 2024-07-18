// Developed by Pluto
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.Events;
using Sora.Utility;

namespace Sora.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private BoolVariable isGameOver;

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }


        public void OnGameOver(Component Invoker, object data)
        {
            isGameOver.value = true;
        }
    }
}