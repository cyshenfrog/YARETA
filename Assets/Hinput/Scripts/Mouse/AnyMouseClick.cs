using System.Collections.Generic;
using System.Linq;

namespace HinputClasses.Internal {
    public class AnyMouseClick : MouseClick {
        // --------------------
        // CONSTRUCTOR
        // --------------------
        
        public AnyMouseClick(string name, Mouse device) :
            base(name, device, null) {
            clicks = new List<MouseClick> {device.leftClick, device.rightClick, device.middleClick};
        }

        
        // --------------------
        // PRIVATE PROPERTIES
        // --------------------

        private readonly List<MouseClick> clicks;
        
        
        // --------------------
        // UPDATE
        // --------------------
        
        protected override bool GetPressed() => clicks.Any(click => click.pressed);
    }
}
