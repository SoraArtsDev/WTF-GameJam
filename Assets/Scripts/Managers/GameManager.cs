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
        int[] slimeCount = { 1, 1, 2 };
        int currentSlimeCount = 0;

        [SerializeField] private BoolVariable isGameOver;

        public int GetCurrentSlimeCount()
        {
            return currentSlimeCount;
        }

        public int GetRequiredSlimeCount()
        {
            return slimeCount[SceneManager.instance.GetcurrentSceneIndex()];
        }
        public void IncrementSlimeCount()
        {
            
            currentSlimeCount++;
            TextManager.instance.UpdateText(currentSlimeCount, GetRequiredSlimeCount());
            Debug.Log("currentSlimeCount : " + currentSlimeCount);
        }

        public bool HaveCollectedRequiredSlime()
        {
            int currentLevel = SceneManager.instance.GetcurrentSceneIndex();
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
            Application.Quit();
            SceneManager.instance.LoadGameScene(2);
        }
    }
}