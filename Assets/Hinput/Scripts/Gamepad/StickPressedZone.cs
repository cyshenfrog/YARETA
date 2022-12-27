﻿namespace HinputClasses {
    /// <summary>
    /// Hinput class representing a stick or D-pad as a button. It is considered pressed if the stick is pushed in any
    /// direction.
    /// </summary>
    public class StickPressedZone : HinputClasses.Internal.StickPressable {
        // --------------------
        // CONSTRUCTOR
        // --------------------

        public StickPressedZone(string name, Stick stick) : 
            base(name, stick) { }

	    
        // --------------------
        // UPDATE
        // --------------------

        protected override bool GetPressed() => stick.distance > Settings.stickPressedZone;
    }
}