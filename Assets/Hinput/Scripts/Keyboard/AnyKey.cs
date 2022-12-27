using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HinputClasses.Internal {
    public class AnyKey : Key {
        // --------------------
        // CONSTRUCTOR
        // --------------------
        
        public AnyKey(Keyboard device) :
            base("", device, "Keyboard_AnyKey", KeyCode.None) {
            allKeys = device.keys;
        }
        
        
        // --------------------
        // PRIVATE PROPERTY
        // --------------------

        private readonly List<Key> allKeys;
        
        
        // --------------------
        // UPDATE
        // --------------------

        protected override bool GetPressed() => allKeys.Any(key => key.pressed);
    }
}