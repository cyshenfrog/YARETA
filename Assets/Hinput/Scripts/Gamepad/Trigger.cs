using HinputClasses.Internal;
using UnityEngine;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing the left or right trigger of a controller.
    /// </summary>
    public class Trigger : HinputClasses.Internal.GamepadPressable {
        // --------------------
    	// CONSTRUCTOR
    	// --------------------
    
    	public Trigger (string name, Gamepad gamepad, int index) : 
    		base(name, gamepad, index) { }
    
    	
    	// --------------------
    	// UPDATE
    	// --------------------
    
        public override void Update() {
	        base.Update();

	        position = GetPosition();
        }
    
        protected virtual float GetPosition() {
	        if (Settings.triggerDeadZone.IsEqualTo(1)) return 0;
	        if (Settings.triggerDeadZone > Settings.triggerMaxZone) return 0;

	        try { return Mathf.Clamp01(
			        gamepad.player.buttons[index].Prel(Settings.triggerDeadZone, Settings.triggerMaxZone));
	        } catch { /* Ignore exceptions here */ }
	        return 0;
        }
        
        protected override bool GetPressed() => position >= Settings.triggerPressedZone;

		
        // --------------------
        // PUBLIC PROPERTY
        // --------------------
        
        /// <summary>
        /// The position of a trigger, between 0 and 1.
        /// </summary>
        public float position { get; private set; }
    }
}