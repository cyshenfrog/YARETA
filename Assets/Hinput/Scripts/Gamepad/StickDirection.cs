﻿using HinputClasses.Internal;

namespace HinputClasses {
    /// <summary>
	/// Hinput class representing a given direction of a stick or D-pad, such as the up or down-left directions.
	/// </summary>
	public class StickDirection : HinputClasses.Internal.StickPressable {
		// --------------------
		// PRIVATE PROPERTY
		// --------------------

		private float angle { get; }


		// --------------------
		// CONSTRUCTOR
		// --------------------

		public StickDirection (string name, float angle, Stick stick) : 
			base(name, stick) {
			this.angle = angle;
		}

		
		// --------------------
		// UPDATE
		// --------------------

		protected override bool GetPressed() => stick.PushedTowards(angle) && stick.distance>Settings.stickPressedZone;
    }
}