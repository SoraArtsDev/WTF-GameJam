// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


namespace Sora.InputSystem
{
	[CreateAssetMenu(fileName = "GameplayInputReader", menuName = "Sora /Input System /GameplayInputReader")]
	public class GameplayInputReader : ScriptableObject, PCKeyBindings.IPlayerMovementActions, PCKeyBindings.IInteractionsActions
	{
		// Movement Events
		public event UnityAction<Vector2> moveEvent;

		// Interaction Events
		public event UnityAction pickUpEvent;

		[Space]
		[SerializeField] private BindingsContainer bindingsContainer;

		public void Enable()
        {
			bindingsContainer.keyBindings.PlayerMovement.SetCallbacks(this);
			bindingsContainer.keyBindings.Interactions.SetCallbacks(this);

			bindingsContainer.keyBindings.Enable();
		}

		public void OnMovement(InputAction.CallbackContext context)
		{
			if (moveEvent != null)
				moveEvent.Invoke(context.ReadValue<Vector2>());
		}

		public void OnPickUp(InputAction.CallbackContext context)
        {
			if (pickUpEvent != null)
				pickUpEvent.Invoke();

        }
	}
}
