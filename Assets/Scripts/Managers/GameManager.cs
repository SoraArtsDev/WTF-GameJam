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
        int[] slimeCount = {0,1,2,3};
        int currentSlimeCount = 0;

        [SerializeField] private BoolVariable isGameOver;

        public void IncrementSlimeCount()
        {
            currentSlimeCount++;
            Debug.Log("currentSlimeCount : " + currentSlimeCount);
        }

        public bool HaveCollectedRequiredSlime(int currentLevel)
        {
            Debug.Log("slimeCount[currentLevel] : " + slimeCount[currentLevel]);
            Debug.Log("currentSlimeCount : " + currentSlimeCount);
            return slimeCount[currentLevel] == currentSlimeCount;
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadGameScene()
        {
            SceneManager.instance.LoadGameScene(1);
        }

        public void OnGameOver(Component Invoker, object data)
        {
            isGameOver.value = true;
            SceneManager.instance.LoadGameScene(2);
        }
    }
}