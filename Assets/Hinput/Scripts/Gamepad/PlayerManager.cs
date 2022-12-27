using System.Collections.Generic;
using UnityEngine;

namespace HinputClasses.Internal {
    // Class coordianting the back-end controls of every gamepad.
    public class PlayerManager : MonoBehaviour {
        [Header("References")]
        public Player defaultPlayer; // Virtual gamepad that is always released
        
        
        // --------------------
        // SINGLETON PATTERN
        // --------------------

        public static PlayerManager instance;

        public void Awake() {
            if (instance == null) instance = this;
            if (instance != this) Destroy(gameObject);
        }
        
        
        // --------------------
        // PLAYER LIST
        // --------------------

        private List<Player> _players;
        public List<Player> players {
            get {
                if (_players == null) {
                    _players = new List<Player>();
                    for (int i=0; i<(int)Settings.amountOfGamepads; i++) _players.Add(defaultPlayer);
                }
                
                return _players;
            }
        }
    }
}