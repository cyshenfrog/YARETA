using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HinputClasses.Internal {
    // Class handling the back-end controls of a gamepad.
    public class Player : MonoBehaviour {
        [Header("State")]
        public List<float> buttons;
        public List<Vector2> sticks;
        
        [Header("References")]
        public PlayerInput playerInput;
        
        
        // --------------------
        // START METHOD
        // --------------------

        private void Start() {
            transform.SetParent(PlayerManager.instance.transform);
            if (playerInput == null) return;
            
            if ((playerInput.devices[0] is UnityEngine.InputSystem.Gamepad) == false) {
                Destroy(gameObject);
                return;
            }

            name = "Player " + playerInput.playerIndex;
            if (playerInput.playerIndex < (int)Settings.amountOfGamepads) {
                Hinput.gamepad[playerInput.playerIndex].type = playerInput.devices[0].layout;
                PlayerManager.instance.players[playerInput.playerIndex] = this;
            } else ErrorMessages.UnsupportedGamepadWarning(playerInput.playerIndex + 1);
        }
        
        
        // --------------------
        // VIBRATION
        // --------------------
        
        public void SetVibration(float leftIntensity, float rightIntensity) {
            if (playerInput == null) return;
            if (playerInput.devices.Count == 0) return;
            
            ((UnityEngine.InputSystem.Gamepad)playerInput.devices[0])
                .SetMotorSpeeds(leftIntensity, rightIntensity);
        }
        
        
        // --------------------
        // INPUT SETTERS
        // --------------------

        public void SetA(InputAction.CallbackContext context) => buttons[0] = context.ReadValue<float>();
        public void SetB(InputAction.CallbackContext context) => buttons[1] = context.ReadValue<float>();
        public void SetX(InputAction.CallbackContext context) => buttons[2] = context.ReadValue<float>();
        public void SetY(InputAction.CallbackContext context) => buttons[3] = context.ReadValue<float>();
        public void SetLeftBumper(InputAction.CallbackContext context) => buttons[4] = context.ReadValue<float>();
        public void SetRightBumper(InputAction.CallbackContext context) => buttons[5] = context.ReadValue<float>();
        public void SetLeftTrigger(InputAction.CallbackContext context) => buttons[6] = context.ReadValue<float>();
        public void SetRightTrigger(InputAction.CallbackContext context) => buttons[7] = context.ReadValue<float>();
        public void SetBack(InputAction.CallbackContext context) => buttons[8] = context.ReadValue<float>();
        public void SetStart(InputAction.CallbackContext context) => buttons[9] = context.ReadValue<float>();
        public void SetLeftStickClick(InputAction.CallbackContext context) => buttons[10] = context.ReadValue<float>();
        public void SetRightStickClick(InputAction.CallbackContext context) => buttons[11] = context.ReadValue<float>();
        
        public void SetLeftStick(InputAction.CallbackContext context) => sticks[0] = context.ReadValue<Vector2>();
        public void SetRightStick(InputAction.CallbackContext context) => sticks[1] = context.ReadValue<Vector2>();
        public void SetDPad(InputAction.CallbackContext context) => sticks[2] = context.ReadValue<Vector2>();
    }
}
