using UnityEngine;

namespace HinputClasses.Internal {
    public static class ErrorMessages {
        // --------------------
        // ERRORS
        // --------------------
    
        public static void FailedSetupError() {
            Debug.LogError("Hinput error: Hinput set up files not found. Please make sure Hinput is located " +
                           "at the root of your Assets folder, and reimport the package if the problem persists.");
        }

        public static void NoCameraFoundError() {
            Debug.LogError ("Hinput error : No camera found !");
        }
    
    
        // --------------------
        // WARNING
        // --------------------

        public static void UnsupportedGamepadWarning(int gamepadIndex) {
            Debug.LogWarning("Hinput Warning: A " + gamepadIndex + "th controller has been connected, but " +
                             "it will not be tracked by Hinput as only "+ (int)Settings.amountOfGamepads + 
                             " gamepads can be tracked at the same time. You can change the maximum amount of " +
                             "gamepads in Settings.");
        }
    }
}