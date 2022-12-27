using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing a keyboard key.
    /// </summary>
    public class Key : Pressable {
        // --------------------
        // PUBLIC PROPERTY
        // --------------------
        
        /// <summary>
        /// The string that shows up in a text box if you press this key on a standard US keyboard.
        /// </summary>
        public readonly string text;
        
        
        // --------------------
        // CONSTRUCTORS
        // --------------------
        
        public Key(string text, Device device, KeyControl keyControl, KeyCode keyCode) :
            base(("Keyboard_" + keyControl.keyCode), device) {
            this.keyControl = keyControl;
            this.keyCode = keyCode;
            this.text = text;
        }
        
        public Key(string text, Device device, String name, KeyCode keyCode) :
            base(name, device) {
            keyControl = new KeyControl();
            this.keyCode = keyCode;
            this.text = text;
        }
        
        
        // --------------------
        // PRIVATE PROPERTIES
        // --------------------
        
        private readonly KeyControl keyControl;
        private UnityEngine.InputSystem.Key key => keyControl.keyCode;
        private readonly KeyCode keyCode;
        
        
        // --------------------
        // UPDATE
        // --------------------

        protected override bool GetPressed() => keyControl.isPressed;
    }
}
