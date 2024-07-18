// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;


namespace Sora
{
    [CreateAssetMenu(fileName = "NewFloatVariable", menuName = "Sora /Variables /Float Variable")]
    public class FloatVariable : ScriptableObject
    {
        public float value;

        // this value is set to "value" every time a the value is changed in the inspector
        private float initValue;
        [Space]
        // if this value is enabled, value is set to initValue when the game is played
        public bool resetDefaultValue;

        /// <summary>
        /// This method is called when this object's values are changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            initValue = value;
        }

        private void OnEnable()
        {
            if (resetDefaultValue)
                value = initValue;
        }
    }
}
