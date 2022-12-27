using UnityEngine.InputSystem.Controls;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing a mouse button.
    /// </summary>
    public class MouseClick : Pressable {
        // --------------------
        // CONSTRUCTOR
        // --------------------

        public MouseClick(string name, Device device, ButtonControl button) : 
            base(name, device) {
            this.button = button;
        }


        // --------------------
        // PRIVATE PROPERTY
        // --------------------
        
        private readonly ButtonControl button;

        
        // --------------------
        // UPDATE
        // --------------------
        
        protected override bool GetPressed() => button.isPressed;
    }
}
