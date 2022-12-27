using UnityEngine;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing the mouse.
    /// </summary>
    public class Mouse : Device {
        // --------------------
        // PUBLIC PROPERTIES
        // --------------------
        
        /// <summary>
        /// The current mouse position, in pixels from the bottom-left of the screen (bottom-left=(0, 0),
        /// top-right=(*screen width*, *screen height*)).
        /// </summary>
        public Vector2 pixelPosition { get; private set; }
        
        /// <summary>
        /// The current mouse position, in percent from the bottom-left of the screen (bottom-left=(0, 0),
        /// top-right=(1, 1)).
        /// </summary>
        public Vector2 position { get; private set; }
        
        /// <summary>
        /// How much the mouse has moved since the last frame, in pixels.
        /// </summary>
        public Vector2 pixelDelta { get; private set; }
        
        /// <summary>
        /// How much the mouse has moved since the last frame, in percent of screen size.
        /// </summary>
        public Vector2 delta { get; private set; }
        
        /// <summary>
        /// The left click button of the mouse.
        /// </summary>
        public readonly MouseClick leftClick;
        
        /// <summary>
        /// The right click button of the mouse.
        /// </summary>
        public readonly MouseClick rightClick;
        
        /// <summary>
        /// The middle click button of the mouse.
        /// </summary>
        public readonly MouseClick middleClick;
        
        /// <summary>
        /// A virtual button returning every mouse click at once.
        /// </summary>
        public readonly MouseClick anyClick;
        
        /// <summary>
        /// The mouse scroll wheel. 
        /// </summary>
        public readonly Scroll scroll;
        
        
        // --------------------
        // PUBLIC METHODS
        // --------------------

        /// <summary>
        /// Returns true if the mouse is over target. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Over(Collider target, bool stopAtFirstObject = true) {
            if (target == null) return false;
            
            if (stopAtFirstObject) return (Physics.Raycast(
                                               Settings.hinputCamera.ScreenPointToRay(pixelPosition), 
                                               out RaycastHit hit) && 
                                           hit.collider == target);
            else return (target.Raycast(
                             Settings.hinputCamera.ScreenPointToRay(pixelPosition), 
                             out RaycastHit hit, 
                             Mathf.Infinity) &&
                         hit.collider == target);
        }

        /// <summary>
        /// Returns true if the mouse is over target. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Over(GameObject target, bool stopAtFirstObject = true) => 
            Over(target.GetComponent<Collider>(), stopAtFirstObject);

        /// <summary>
        /// Returns true if the mouse is over target. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Over(Component target, bool stopAtFirstObject = true) => 
            Over(target.GetComponent<Collider>(), stopAtFirstObject);
        
        /// <summary>
        /// Returns true if the mouse has clicked target this frame. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Clicked(Collider target, bool stopAtFirstObject = true) => 
            leftClick.justPressed && Over(target, stopAtFirstObject);

        /// <summary>
        /// Returns true if the mouse has clicked target this frame. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Clicked(GameObject target, bool stopAtFirstObject = true) =>
            Clicked(target.GetComponent<Collider>(), stopAtFirstObject);

        /// <summary>
        /// Returns true if the mouse has clicked target this frame. Returns false otherwise.<BR/> <BR/>
        /// Add a second argument worth false to allow Hinput to detect target even if it is in the background,
        /// or hidden by other objects. <BR/> <BR/>
        /// This method only works with Colliders. It won't detect objects that don't have a collider attached, and
        /// things like UI elements.
        /// </summary>
        public bool Clicked(Component target, bool stopAtFirstObject = true) =>
            Clicked(target.GetComponent<Collider>(), stopAtFirstObject);
        
        
        // --------------------
        // CONSTRUCTOR
        // --------------------

        public Mouse() {
            name = "Mouse";
            
            leftClick = new MouseClick("Mouse_LeftClick", this, UnityEngine.InputSystem.Mouse.current.leftButton);
            rightClick = new MouseClick("Mouse_RightClick", this, UnityEngine.InputSystem.Mouse.current.rightButton);
            middleClick = new MouseClick("Mouse_MiddleClick", this, UnityEngine.InputSystem.Mouse.current.middleButton);
            anyClick = new HinputClasses.Internal.AnyMouseClick("Mouse_AnyClick", this);
            
            scroll = new Scroll(UnityEngine.InputSystem.Mouse.current.scroll.y);
        }
        
        
        // --------------------
        // UPDATE
        // --------------------

        /// <summary>
        /// Hinput internal method.
        /// </summary>
        public void Update() {
            if (!isEnabled) return;
            
            leftClick.Update();
            rightClick.Update();
            middleClick.Update();
            
            scroll.Update();
        
            UpdatePosition();
            UpdateDelta();
        }
        
        private void UpdatePosition() {
            pixelPosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            position = new Vector2(pixelPosition.x/Screen.width, pixelPosition.y/Screen.height);
        }

        private void UpdateDelta() {
            pixelDelta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();
            delta = new Vector2(pixelDelta.x/Screen.width, pixelDelta.y/Screen.height);
        }
    }
}
