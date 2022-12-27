namespace HinputClasses.Internal {
    // A pressable from a gamepad, such as a button, a trigger or AnyInput.
    public abstract class GamepadPressable : Pressable {
        // --------------------
        // ID
        // --------------------

        /// <summary>
        /// The index of a button on its gamepad.
        /// </summary>
        public readonly int index;
        
        /// <summary>
        /// The gamepad an input is attached to.
        /// </summary>
        public readonly Gamepad gamepad;
	
	
        // --------------------
        // CONSTRUCTOR
        // --------------------
        protected GamepadPressable(string name, Gamepad gamepad, int index) :
            base(gamepad.name + "_" + name, gamepad) {
            this.index = index;
            this.gamepad = gamepad;
        }
    }
}
