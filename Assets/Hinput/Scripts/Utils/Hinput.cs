// Author : Henri Couvreur for hiloqo, 2020
// Contact : hello@hinput.co

using System.Collections.Generic;

/// <summary>
/// The main class of the Hinput package, used to access gamepads.
/// </summary>
public static class Hinput {
	// --------------------
	// GAMEPAD
	// --------------------

	private static List<HinputClasses.Gamepad> _gamepad;
	/// <summary>
	/// The list of Hinput gamepads.<br/> <br/>
	///
	/// By default, Hinput tracks 8 gamepads, labelled 0 to 7.
	/// </summary>
	public static List<HinputClasses.Gamepad> gamepad { 
		get {
			HinputClasses.Internal.Updater.CheckInstance();
			if (_gamepad == null) {
				_gamepad = new List<HinputClasses.Gamepad>();
				for (int i=0; i<(int)HinputClasses.Settings.amountOfGamepads; i++) 
					_gamepad.Add(new HinputClasses.Gamepad(i));
			}

			return _gamepad; 
		} 
	}
	
	
	// --------------------
	// ANYGAMEPAD
	// --------------------

	private static HinputClasses.Gamepad _anyGamepad;
	/// <summary>
	/// A virtual gamepad that returns the inputs of every gamepad at once.<br/> <br/>
	///
	/// A button is pressed on AnyGamepad if there is at least one gamepad on which it is pressed. It is released on
	/// AnyGamepad if it is released on all gamepads.<br/> 
	/// The position of a stick on AnyGamepad is the average position of pushed sticks of that type on all
	/// gamepads.<br/> 
	/// Vibrating AnyGamepad vibrates all gamepads.<br/> <br/>
	/// 
	/// Examples: <br/>
	/// - If player 1 pushed their A button and player 2 pushed their B button, both the A and the B button of
	/// AnyGamepad will be pressed.<br/>
	/// - If both player 1 and player 2 hold their A button, and player 1 releases it, the A button of AnyGamepad will
	/// still be considered pressed, and will NOT trigger a justReleased event.<br/>
	/// - If player 1 pushed their left trigger by 0.2 and player 2 pushed theirs by 0.6, the left trigger of
	/// AnyGamepad will have a position of 0.6.<br/>
	/// - If player 1 positioned their right stick at (-0.2, 0.9) and player 2 has theirs at (0, 0), the
	/// position of the right stick of AnyGamepad will be (-0.2, 0.9).<br/>
	/// - If player 1 positioned their right stick at (-0.2, 0.9) and player 2 has theirs at (0.6, 0.3), the
	/// position of the right stick of AnyGamepad will be the average of both positions, (0.2, 0.6).
	/// </summary>
	public static HinputClasses.Gamepad anyGamepad { 
		get { 
			HinputClasses.Internal.Updater.CheckInstance();
			if (_anyGamepad == null) _anyGamepad = new HinputClasses.Internal.AnyGamepad();
			return _anyGamepad; 
		}
	}


	// --------------------
	// ANYINPUT
	// --------------------
	
	/// <summary>
	/// A virtual button that returns every input of every gamepad at once.
	/// AnyInput is considered pressed if at least one input on that gamepad is pressed.
	/// AnyInput is considered released if every input on that gamepad is released.
	/// </summary>
	public static HinputClasses.Pressable anyInput => anyGamepad.anyInput;


	// --------------------
	// MOUSE
	// --------------------

	private static HinputClasses.Mouse _mouse;
	/// <summary>
	/// The mouse.
	/// </summary>
	public static HinputClasses.Mouse mouse {
		get {
			HinputClasses.Internal.Updater.CheckInstance();
			if (_mouse == null) _mouse = new HinputClasses.Mouse();
			return _mouse;
		}
	}


	// --------------------
	// KEYBOARD
	// --------------------

	private static HinputClasses.Keyboard _keyboard;
	/// <summary>
	/// The keyboard.<BR/> <BR/>
	/// The properties of this class refer to the keys as positioned on a US keyboard, regardless of current keyboard
	/// layout. For instance, Hinput.keyboard.A always refers to the leftmost key of the second letter row. It maps
	/// to the A key of a standard US keyboard, but the Q key of a French keyboard.
	/// </summary>
	public static HinputClasses.Keyboard keyboard {
		get {
			HinputClasses.Internal.Updater.CheckInstance();
			if (_keyboard == null) _keyboard = new HinputClasses.Keyboard();
			return _keyboard;
		}
	}
}