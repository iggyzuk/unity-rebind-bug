using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// bug: notice how after rebinding the key that was used to initiate it is still stuck in its down state
// this seems to only happen on the keyboard, I tested with PS5 gamepad and everything works as expected 
// you can see the state of keys by going into Window > Analysis > Input Debugger > Devices > Keyboard
public class MyInputWithRebinding : MonoBehaviour {
	
	public InputActionReference actionReference;
	
	// note: is there a better way to link to a specific binding?
	public string bindingId; 

	private InputActionRebindingExtensions.RebindingOperation rebindOp;

	private void Update() {
		if (actionReference.action.WasPerformedThisFrame()) {
			Debug.Log($"Pressed ({Time.time})");
			StartRebind();
		}
	}

	private void StartRebind() {

		Debug.Log("Rebind: start");
		
		var action = actionReference.action;
		action.Disable();

		void CleanUp() {
			Debug.Log($"Rebind: cleanup");
			action.Enable();
			rebindOp?.Dispose();
			
			// feels like calling action.Reset(); should do the trick here
			// but unfortunately it doesn't do anything
		}

		// find the binding index
		var binding = actionReference.action.bindings.FirstOrDefault(x => x.id == new Guid(bindingId));
		var bindingIndex = action.bindings.IndexOf(x => x.id == binding.id);

		// listen for a key to rebind to
		rebindOp = action.PerformInteractiveRebinding(bindingIndex)
			.WithControlsExcluding("Mouse")
			.OnMatchWaitForAnother(0.2f)
			.OnCancel(
				op => {
					Debug.Log($"Rebind: cancel");
					CleanUp();
				})
			.OnComplete(
				op => {
					Debug.Log($"Rebind: complete: {op.action.GetBindingDisplayString(bindingIndex)}");
					CleanUp();
				})
			.Start();
	}
}