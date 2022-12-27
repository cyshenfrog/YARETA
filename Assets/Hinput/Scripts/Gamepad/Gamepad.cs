using System.Collections.Generic;
using UnityEngine;
using HinputClasses.Internal;

namespace HinputClasses {
	/// <summary>
	/// Hinput class representing a gamepad.
	/// </summary>
	public class Gamepad : Device {
		// --------------------
		// ID
		// --------------------

		/// <summary>
		/// The index of a gamepad in the gamepad list of Hinput. AnyGamepad returns -1.
		/// </summary>
		public int index { get; protected set; }

		/// <summary>
		/// Hinput internal property.
		/// </summary>
		public HinputClasses.Internal.Player player {
			get {
				if (HinputClasses.Internal.PlayerManager.instance.players.Count > index && index >= 0) {
					return HinputClasses.Internal.PlayerManager.instance.players[index];
				} else return HinputClasses.Internal.PlayerManager.instance.defaultPlayer;
			} 
		}

		/// <summary>
		/// Returns true if a gamepad is currently connected. Returns false otherwise.<br/> <br/>
		/// On AnyGamepad, returns true if at least one gamepad is currently connected. Returns false otherwise.
		/// </summary>
		public virtual bool isConnected => (player.playerInput != null && player.playerInput.devices.Count > 0);

		/// <summary>
		/// The type of a gamepad, like “XInputControllerWindows”, “AnyGamepad” or “DualShock4GamepadHID.
		/// </summary>
		public virtual string type { get; set; }
		
		
		// --------------------
		// ENABLED
		// --------------------

		/// <summary>
		/// Disable a gamepad so that Hinput stops tracking it. <br/> <br/>
		/// Calling this method on AnyGamepad only disables AnyGamepad.
		/// </summary>
		public override void Disable() {
			buttons.ForEach(button => button.Reset());
			sticks.ForEach(stick => stick.Reset());
			anyInput.Reset();
			StopVibration();
			
			isEnabled = false;
		}


		// --------------------
		// CONSTRUCTORS
		// --------------------

		protected Gamepad() { }

		public Gamepad (int index) {
			this.index = index;
			name = "Gamepad" + index;
			
			A = new Button ("A", this, 0); 
			B = new Button ("B", this, 1);
			X = new Button ("X", this, 2);
			Y = new Button ("Y", this, 3);
			
			leftBumper = new Button ("LeftBumper", this, 4);
			rightBumper = new Button ("RightBumper", this, 5);
			leftTrigger = new Trigger ("LeftTrigger", this, 6);
			rightTrigger = new Trigger ("RightTrigger", this, 7);
			back = new Button ("Back", this, 8);
			start = new Button ("Start", this, 9);
			
			leftStickClick = new Button ("LeftStickClick", this, 10);
			rightStickClick = new Button ("RightStickClick", this, 11);
			
			leftStick = new Stick ("LeftStick", this, 0);
			rightStick = new Stick ("RightStick", this, 1);
			dPad = new Stick ("DPad", this, 2);
			
			vibration = new HinputClasses.Internal.Vibration (this);
			
			SetUp();
		}
			
		protected void SetUp() {
			anyInput = new HinputClasses.Internal.AnyInput("AnyInput", this);
			
			sticks = new List<Stick> { leftStick, rightStick, dPad };
			buttons = new List<Pressable> {
				A, B, X, Y,
				leftBumper, rightBumper, leftTrigger, rightTrigger,
				back, start, leftStickClick, rightStickClick
			};
		}

		
		// --------------------
		// UPDATE
		// --------------------

		/// <summary>
		/// Hinput internal method.
		/// </summary>
		public void Update () {
			if (isEnabled == false) return;

			buttons.ForEach(button => button.Update());
			sticks.ForEach(stick => stick.Update());
			anyInput.Update();
			
			SpecificUpdate();
		}

		protected virtual void SpecificUpdate() {
			vibration.Update();
		}

		
		// --------------------
		// PUBLIC PROPERTIES
		// --------------------

		/// <summary>
		/// The A button of a gamepad.
		/// </summary>
		public Button A { get; protected set; }

		/// <summary>
		/// The B button of a gamepad.
		/// </summary>
		public Button B { get; protected set; }

		/// <summary>
		/// The X button of a gamepad.
		/// </summary>
		public Button X { get; protected set; }

		/// <summary>
		/// The Y button of a gamepad.
		/// </summary>
		public Button Y { get; protected set; }

		/// <summary>
		/// The left bumper of a gamepad.
		/// </summary>
		public Button leftBumper { get; protected set; }

		/// <summary>
		/// The right bumper of a gamepad.
		/// </summary>
		public Button rightBumper { get; protected set; }

		/// <summary>
		/// The back button of a gamepad.
		/// </summary>
		public Button back { get; protected set; }

		/// <summary>
		/// The start button of a gamepad.
		/// </summary>
		public Button start { get; protected set; }

		/// <summary>
		/// The left stick click of a gamepad.
		/// </summary>
		public Button leftStickClick { get; protected set; }

		/// <summary>
		/// The right stick click of a gamepad.
		/// </summary>
		public Button rightStickClick { get; protected set; }

		/// <summary>
		/// The left trigger of a gamepad.
		/// </summary>
		public Trigger leftTrigger { get; protected set; }
		
		/// <summary>
		/// The right trigger of a gamepad.
		/// </summary>
		public Trigger rightTrigger { get; protected set; }

		/// <summary>
		/// The left stick of a gamepad.
		/// </summary>
		public Stick leftStick { get; protected set; }

		/// <summary>
		/// The right stick of a gamepad.
		/// </summary>
		public Stick rightStick { get; protected set; }
		
		/// <summary>
		/// The D-pad of a gamepad.
		/// </summary>
		public Stick dPad { get; protected set; }

		/// <summary>
		/// The virtual button that returns every input of a gamepad at once.<br/> <br/>
		/// AnyInput is considered pressed if at least one input on that gamepad is pressed.
		/// AnyInput is considered released if every input on that gamepad is released.
		/// </summary>
		public Pressable anyInput { get; private set; }

		/// <summary>
		/// The list containing the sticks of a gamepad, in the following order : { leftStick, rightStick, dPad }
		/// </summary>
		public List<Stick> sticks { get; private set; }

		/// <summary>
		/// The list containing the buttons of a gamepad, in the following order : { A, B, X, Y, left bumper, right
		/// bumper, left trigger, right trigger, back, start, left stick click, right stick click }
		/// </summary>
		public List<Pressable> buttons { get; private set; }

		// --------------------
		// VIBRATION
		// --------------------
		
		private readonly HinputClasses.Internal.Vibration vibration;

		/// <summary>
		/// The intensity at which the left motor of a gamepad is currently vibrating.<br/> <br/>
		/// On AnyGamepad, returns -1.
		/// </summary>
		public virtual float leftVibration {
			get {
				if (vibration.currentLeft.IsEqualTo(0)) return 0;
				return vibration.currentLeft;
			}
		}

		/// <summary>
		/// The intensity at which the right motor of a gamepad is currently vibrating.<br/> <br/>
		/// On AnyGamepad, returns -1.
		/// </summary>
		public virtual float rightVibration {
			get {
				if (vibration.currentRight.IsEqualTo(0)) return 0;
				return vibration.currentRight;
			}
		}

		/// <summary>
		/// Vibrate a gamepad.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate() {
			vibration.Vibrate(
				Settings.vibrationDefaultLeftIntensity, 
				Settings.vibrationDefaultRightIntensity, 
				Settings.vibrationDefaultDuration);
		}

		/// <summary>
		/// Vibrate a gamepad for duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float duration) {
			vibration.Vibrate(
				Settings.vibrationDefaultLeftIntensity, 
				Settings.vibrationDefaultRightIntensity, 
				duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor, and an intensity of rightIntensity
		/// on the right motor.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float leftIntensity, float rightIntensity) {
			vibration.Vibrate(
				leftIntensity, 
				rightIntensity, 
				Settings.vibrationDefaultDuration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor and an intensity of rightIntensity
		/// on the right motor, for duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float leftIntensity, float rightIntensity, float duration) {
			vibration.Vibrate(
				leftIntensity, 
				rightIntensity, 
				duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an intensity over time based on an animation curve.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(AnimationCurve curve) {
			vibration.Vibrate(curve, curve);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an intensity over time based on two animation curves, one for the left side and one
		/// for the right side.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(AnimationCurve leftCurve, AnimationCurve rightCurve) {
			vibration.Vibrate(leftCurve, rightCurve);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset) {
			vibration.Vibrate(vibrationPreset, 1, 1, 1);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The duration of the preset is multiplied by duration.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float duration) {
			vibration.Vibrate(vibrationPreset, 1, 1, duration);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The left intensity of the preset is multiplied by leftIntensity, and its right intensity is multiplied by
		/// rightIntensity.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity) {
			vibration.Vibrate(vibrationPreset, leftIntensity, rightIntensity, 1);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The left intensity of the preset is multiplied by leftIntensity, its right intensity is multiplied by
		/// rightIntensity, and its duration is multiplied by duration.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity, 
			float duration) {
			vibration.Vibrate(vibrationPreset, leftIntensity, rightIntensity, duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor, and an intensity of rightIntensity
		/// on the right motor, FOREVER. Don't forget to call StopVibration!<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void VibrateAdvanced(float leftIntensity, float rightIntensity) {
			vibration.VibrateAdvanced(leftIntensity, rightIntensity);
		}

		/// <summary>
		/// Stop all vibrations on a gamepad immediately.<br/> <br/>
		/// Calling this on AnyGamepad stops all gamepads.
		/// </summary>
		public virtual void StopVibration () {
			vibration.StopVibration(0);
		}

		/// <summary>
		/// Stop all vibrations on a gamepad progressively over duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad stops all gamepads.
		/// </summary>
		public virtual void StopVibration (float duration) {
			vibration.StopVibration(duration);
		}
	}
}