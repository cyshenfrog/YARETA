using HinputClasses.Internal;
using UnityEngine.InputSystem.Controls;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing the scroll wheel of the mouse.
    /// </summary>
    public class Scroll {
        // --------------------
        // PUBLIC PROPERTIES
        // --------------------
        
        /// <summary>
        /// The current position of the scroll wheel (scrolling down=-1, scrolling up=1, not scrolling=0).
        /// </summary>
        public float position;

        /// <summary>
        /// Returns true if the scroll wheel is scrolling up. Returns false otherwise.
        /// </summary>
        public bool up => position > 0;
        
        /// <summary>
        /// Returns false if the scroll wheel is scrolling down. Returns false otherwise.
        /// </summary>
        public bool down => position < 0;
        
        
        // --------------------
        // IMPLICIT CONVERSION
        // --------------------

        public static implicit operator float(Scroll scroll) => scroll.position;
        
        
        // --------------------
        // PRIVATE PROPERTIES
        // --------------------

        private AxisControl axis;
        
        
        // --------------------
        // CONSTRUCTOR
        // --------------------

        public Scroll(AxisControl axis) => this.axis = axis;
        
        
        // --------------------
        // UPDATE
        // --------------------

        /// <summary>
        /// Hinput internal method.
        /// </summary>
        public void Update() => position = GetPosition();

        private float GetPosition() {
            float value = axis.ReadValue();
            if (value.IsEqualTo(0)) return 0;
            else if (value < 0) return -1;
            else return 1;
        }
    }
}
