using HinputClasses.Internal;

namespace HinputClasses {
	/// <summary>
	/// Hinput class representing a physical button of a controller, such as the A button, a bumper or a stick click.
	/// </summary>
	public class Button : HinputClasses.Internal.GamepadPressable {
		// --------------------
		// CONSTRUCTOR
		// --------------------

		public Button(string name, Gamepad gamepad, int index) : 
			base(name, gamepad, index) { }

	
		// --------------------
		// UPDATE
		// --------------------

		protected override bool GetPressed() {
			try { return gamepad.player.buttons[index].IsEqualTo(1); } 
			catch { /* Ignore exceptions here */ }
			return false; 
		}
	}
}