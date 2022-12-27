using System.Linq;
using UnityEngine;

namespace HinputClasses.Internal {
    /// <summary>
    /// Hinput class representing every gamepad at once.
    /// </summary>
    public class AnyGamepad : Gamepad {
        // --------------------
        // ID
        // --------------------

        public override string type => "AnyGamepad";
        public override bool isConnected => Hinput.gamepad.Any(gamepad => gamepad.isConnected);


        // --------------------
        // CONSTRUCTOR
        // --------------------

        public AnyGamepad() {
            index = -1;
            name = "AnyGamepad";
            isEnabled = true;
			
            A = new AnyGamepadButton ("A", this, 0); 
            B = new AnyGamepadButton ("B", this, 1);
            X = new AnyGamepadButton ("X", this, 2);
            Y = new AnyGamepadButton ("Y", this, 3);
			
            leftBumper = new AnyGamepadButton ("LeftBumper", this, 4);
            rightBumper = new AnyGamepadButton ("RightBumper", this, 5);
            leftTrigger = new AnyGamepadTrigger ("LeftTrigger", this, 6);
            rightTrigger = new AnyGamepadTrigger ("RightTrigger", this, 7);
            back = new AnyGamepadButton ("Back", this, 8);
            start = new AnyGamepadButton ("Start", this, 9);
			
            leftStickClick = new AnyGamepadButton ("LeftStickClick", this, 10);
            rightStickClick = new AnyGamepadButton ("RightStickClick", this, 11);
            
            leftStick = new AnyGamepadStick("LeftStick", this, 0);
            rightStick = new AnyGamepadStick("RightStick", this, 1);
            dPad = new AnyGamepadStick("DPad", this, 2);

            SetUp();
        }
        

		// --------------------
        // UPDATE
        // --------------------

        protected override void SpecificUpdate() { }


        // --------------------
		// VIBRATION
		// --------------------

		public override float leftVibration => -1;
		public override float rightVibration => -1;

		public override void Vibrate() => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate());

		public override void Vibrate(float duration) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(duration));
		
		public override void Vibrate(float leftIntensity, float rightIntensity) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(leftIntensity, rightIntensity));
		
		public override void Vibrate(float leftIntensity, float rightIntensity, float duration) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(leftIntensity, rightIntensity, duration));
		
		public override void Vibrate(AnimationCurve curve) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(curve));
		
		public override void Vibrate(AnimationCurve leftCurve, AnimationCurve rightCurve) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(leftCurve, rightCurve));

		public override void Vibrate(VibrationPreset vibrationPreset) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(vibrationPreset));

		public override void Vibrate(VibrationPreset vibrationPreset, float duration) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(vibrationPreset, duration));

		public override void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(vibrationPreset, leftIntensity, rightIntensity));

		public override void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity, 
			float duration) =>
			Hinput.gamepad.ForEach(gamepad => gamepad.Vibrate(vibrationPreset, leftIntensity, rightIntensity, duration));
		
		public override void VibrateAdvanced(float leftIntensity, float rightIntensity) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.VibrateAdvanced(leftIntensity, rightIntensity));

		public override void StopVibration () => 
			Hinput.gamepad.ForEach(gamepad => gamepad.StopVibration());

		public override void StopVibration (float duration) => 
			Hinput.gamepad.ForEach(gamepad => gamepad.StopVibration(duration));
    }
}