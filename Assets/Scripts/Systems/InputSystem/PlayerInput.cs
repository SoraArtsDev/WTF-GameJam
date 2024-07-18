// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;
using Sora.InputSystem;

namespace Sora.Game
{
	public enum E_TestPlayerStates
    {
		IDLE,
		WALKING,
		RUNNING
    }

    public class PlayerInput : MonoBehaviour
    {
		[Header("Test Variables")]
		[SerializeField] private float test;
		public GameObject testGO;
		public Utility.VolatileVariable<E_TestPlayerStates> playerState;
		public BoolVariable notIdle;

		[Header("Test Input")]
		// sample InputSytem usage
		[SerializeField] GameplayInputReader gpInputReader;
		public Vector2 movementDir;
		[SerializeField] private float playerMoveSpeed;

		private void OnEnable()
        {
			// Input system usage
			gpInputReader.moveEvent += OnMove;

			// Custom Varible Usage
			playerState.OnVariableChange += SampleCustomVariableStateChange;
        }

        void Update()
		{
			if (movementDir.magnitude != 0)
				transform.Translate(Time.deltaTime * playerMoveSpeed * new Vector3(movementDir.x, 0.0f, movementDir.y));
		}

		private void OnMove(Vector2 val)
        {
			movementDir = val;
        }

		private void SampleCustomVariableStateChange(E_TestPlayerStates state)
        {
			if (state == E_TestPlayerStates.RUNNING)
				Debug.Log("Run");
			else if (state == E_TestPlayerStates.WALKING)
				Debug.Log("Walk");
			else if (notIdle.value && state == E_TestPlayerStates.IDLE)
				Debug.Log("Idle");
		}
	}
}

