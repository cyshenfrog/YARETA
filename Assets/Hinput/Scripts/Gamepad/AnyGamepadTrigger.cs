using System.Collections.Generic;
using System.Linq;

namespace HinputClasses.Internal {
    public class AnyGamepadTrigger : Trigger {
        // --------------------
        // CONSTRUCTOR
        // --------------------
        
        public AnyGamepadTrigger(string name, Gamepad gamepad, int index) :
            base(name, gamepad, index) {
            triggers = Hinput.gamepad.Select(g => (HinputClasses.Trigger)g.buttons[index]).ToList();
        }


        // --------------------
        // BUTTONS
        // --------------------

        // Every stick of this type
        private readonly List<HinputClasses.Trigger> triggers;


        // --------------------
        // UPDATE
        // --------------------

        protected override float GetPosition() => triggers.Select(button => button.position).Max();
    }
}