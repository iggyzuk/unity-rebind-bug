using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class MyInputManager : MonoBehaviour {

	public PlayerInputManager manager;
	public GameObject prefab;

	private void Awake() {
		InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
		InputUser.listenForUnpairedDeviceActivity++;

		manager.onPlayerJoined += OnPlayerJoined;
	}

	private void OnPlayerJoined (PlayerInput playerInput) {
		Debug.Log("MyInputManager: player joined");
	}

	private void OnUnpairedDeviceUsed (InputControl control, InputEventPtr arg2) {
		Debug.Log("MyInputManager: unused device used");

		if (control is ButtonControl keyboard) {

			Debug.Log("MyInputManager: button-control used");

			PlayerInput.Instantiate(
				prefab: prefab,
				playerIndex: 0,
				pairWithDevice: control.device
			);

			InputUser.listenForUnpairedDeviceActivity--;
		}
	}
}