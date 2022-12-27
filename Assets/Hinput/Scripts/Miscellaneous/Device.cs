namespace HinputClasses {
    /// <summary>
    /// Hinput abstract class representing a type of device, like a gamepad, a mouse or a keyboard.
    /// </summary>
    public abstract class Device {
        /// <summary>
        /// The name of a device, like "Gamepad0", "Keyboard" or "AnyGamepad".
        /// </summary>
        public string name;

        /// <summary>
        /// Returns true if a device is being tracked by Hinput. Returns false otherwise.<br/> <br/>
        /// On AnyGamepad, returns true if AnyGamepad is enabled (this does NOT give any information on regular
        /// gamepads). Returns false otherwise.
        /// </summary>
        public bool isEnabled = true;

        /// <summary>
        /// Enable a device so that Hinput starts tracking it. <br/> <br/>
        /// Calling this method on AnyGamepad only enables AnyGamepad.
        /// </summary>
        public virtual void Enable() => isEnabled = true;
    
        /// <summary>
        /// Disable a device so that Hinput stops tracking it. <br/> <br/>
        /// Calling this method on AnyGamepad only disables AnyGamepad.
        /// </summary>
        public virtual void Disable() => isEnabled = false;
    }
}
