using UnityEngine;

namespace HinputClasses {
    /// <summary>
	/// Hinput abstract class representing anything that can be pressed and released. Buttons, stick clicks, triggers
	/// and stick directions are Pressable.
	/// </summary>
	public abstract class Pressable {
		// --------------------
		// ID
		// --------------------

		/// <summary>
		/// The name of an input, like “Gamepad0_A”, "Keyboard_K", “AnyGamepad_RightStick_Up” or “Mouse_AnyClick”.
		/// </summary>
		public readonly string name;

		/// <summary>
		/// The device an input is attached to.
		/// </summary>
		public readonly Device device;
		
		
		// --------------------
		// ENABLED
		// --------------------
		
		/// <summary>
		/// Returns true if an input is being tracked by Hinput. Returns false otherwise.<br/> <br/>
		/// On AnyInput, returns true if AnyInput is enabled (this does NOT give any information on regular inputs).
		/// Returns false otherwise.
		/// </summary>
		public bool isEnabled { get; private set; }
		
		/// <summary>
		/// Enable an input so that Hinput starts tracking it.<br/> <br/>
		/// Calling this method on AnyInput only enables AnyInput.
		/// </summary>
		public void Enable() => isEnabled = true;

		/// <summary>
		/// Disable an input so that Hinput stops tracking it. <br/> <br/>
		/// Calling this method on AnyInput only disables AnyInput.
		/// </summary>
		public void Disable() {
			Reset();
			isEnabled = false;
		}

		/// <summary>
		/// Hinput internal method.
		/// </summary>
		public void Reset() => penultimatePressStart = 0f;

		
		// --------------------
		// IMPLICIT CONVERSIONS
		// --------------------

		public static implicit operator bool(Pressable pressable) => (bool)(Press)pressable;

		public static implicit operator Press(Pressable pressable) {
			if (pressable == null) return null;
			if (Settings.defaultPressType == Settings.DefaultPressTypes.LongPress) return pressable.longPress;
			if (Settings.defaultPressType == Settings.DefaultPressTypes.DoublePress) return pressable.doublePress;
			return pressable.simplePress;
		}
		
		/// <summary>
		/// Returns true if an input is pressed. Returns false otherwise.
		/// </summary>
		public bool pressed => ((Press)this).pressed;
		
		/// <summary>
		/// Returns true if an input is released. Returns false otherwise.
		/// </summary>
		public bool released => ((Press)this).released;
		
		/// <summary>
		/// Returns true if an input has been pressed this frame. Returns false otherwise.
		/// </summary>
		public bool justPressed => ((Press)this).justPressed;
		
		/// <summary>
		/// Returns true if an input has been released this frame. Returns false otherwise.
		/// </summary>
		public bool justReleased => ((Press)this).justReleased;
		
		/// <summary>
		/// How long an input has been pressed (0 if it is released).
		/// </summary>
		public float pressDuration => ((Press)this).pressDuration;

		/// <summary>
		/// How long an input has been released (0 if it is pressed).
		/// </summary>
		public float releaseDuration => ((Press)this).releaseDuration;


		// --------------------
		// CONSTRUCTOR
		// --------------------

		protected Pressable(string name, Device device) {
			this.name = name;
			this.device = device;
			isEnabled = true;
			
			simplePress = new Press(this);
			longPress = new Press(this);
			doublePress = new Press(this);
		}

		
		// --------------------
		// PRIVATE PROPERTIES
		// --------------------

		private bool isPressed = false;
		private float lastPressStart = 0f;
		private float penultimatePressStart = 0f;

		
		// --------------------
		// UPDATE
		// --------------------
		protected abstract bool GetPressed();

		/// <summary>
		/// Hinput internal method.
		/// </summary>
		public virtual void Update () {
			if (!isEnabled) return;
			
			bool prevPressed = isPressed;
			isPressed = GetPressed();

			if (isPressed && !prevPressed) {
				penultimatePressStart = lastPressStart;
				lastPressStart = Time.unscaledTime;		
			}
			
			simplePress.Update(isPressed);
			longPress.Update(isPressed && (Time.unscaledTime - lastPressStart > Settings.longPressDuration));
			doublePress.Update(isPressed && (lastPressStart - penultimatePressStart < Settings.doublePressDuration));
		}

		
		// --------------------
		// PUBLIC PROPERTIES
		// --------------------

		/// <summary>
		/// Considered pressed whenever an input is pressed.
		/// </summary>
		public readonly Press simplePress;

		/// <summary>
		/// Considered pressed when an input has been pressed twice in a row.
		/// </summary>
		public readonly Press doublePress;
		
		/// <summary>
		/// Considered pressed when an input has been pressed for a long time.
		/// </summary>
		public readonly Press longPress;
	}
}