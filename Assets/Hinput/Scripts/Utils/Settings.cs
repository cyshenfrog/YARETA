using UnityEngine;

namespace HinputClasses {
    /// <summary>
	/// Hinput class responsible for handling settings.<br/> <br/>
    /// Can be attached to a gameobject to expose settings. Otherwise it will automatically be instantiated at runtime
    /// the first time Hinput is called, with default settings.
	/// </summary>
	public class Settings : MonoBehaviour {
		// --------------------
		// SINGLETON PATTERN
		// --------------------

		//The instance of Settings. Assigned when first called.
		private static Settings _instance;
		public static Settings instance { 
			get {
				CheckInstance();
				return _instance;
			} 
		}

		private static void CheckInstance() {
			if (_instance != null) return;
			
			GameObject go = new GameObject {name = "Hinput Settings"};
			_instance = go.AddComponent<Settings>();
		}

		private void Awake () {
			if (_instance == null) _instance = this;
			if (_instance != this) Destroy(this);
			DontDestroyOnLoad (this);
		}


		// --------------------
		// GAMEPADS
		// --------------------

		public enum AmountOfGamepads { Eight = 8, Sixteen = 16, ThirtyTwo = 32, SixtyFour = 64 }
		[Header("Gamepads")]

		[SerializeField]
		[Tooltip("The maximum amount of gamepads that can be tracked at once by Hinput.")]
		private AmountOfGamepads _amountOfGamepads = AmountOfGamepads.Eight;
		/// <summary>
		/// The maximum amount of gamepads that can be tracked at once by Hinput.
		/// </summary>
		public static AmountOfGamepads amountOfGamepads { 
			get => instance._amountOfGamepads;
		}


		// --------------------
		// IMPLICIT CONVERSION
		// --------------------

		public enum DefaultPressTypes { SimplePress, DoublePress, LongPress }
		[Header("Implicit Conversion")]

		[SerializeField]
		[Tooltip("The default conversion of Pressable to Press values\n\n"+
		         "Determines how Hinput interprets buttons, triggers and stick directions when the type of Press to " +
		         "use is not specified.")]
		private DefaultPressTypes _defaultPressType = DefaultPressTypes.SimplePress;
		/// <summary>
		/// The default conversion of Pressable to boolean values<br/> <br/>
		/// Determines how Hinput interprets buttons, triggers and stick directions when the type of Press to use is
		/// not specified.
		/// </summary>
		public static DefaultPressTypes defaultPressType { 
			get => instance._defaultPressType;
			set => instance._defaultPressType = value;
		}
		
		public enum DefaultPressFeatures { Pressed, JustPressed, Released, JustReleased }
		[SerializeField]
		[Tooltip("The default conversion of Press and Pressable to boolean values\n\n"+
		         "Determines how Hinput interprets buttons, triggers and stick directions when when the feature to " +
		         "use is not specified.")]
		private DefaultPressFeatures _defaultPressFeature = DefaultPressFeatures.Pressed;
		/// <summary>
		/// The default conversion of Press and Pressable to boolean values<br/> <br/>
		/// Determines how Hinput interprets buttons, triggers and stick directions when when the feature to use is
		/// not specified.
		/// </summary>
		public static DefaultPressFeatures defaultPressFeature { 
			get => instance._defaultPressFeature;
			set => instance._defaultPressFeature = value;
		}


		// --------------------
		// PRESSES
		// --------------------
		
		[Header("Presses")]

		[SerializeField]
		[Range(0,2)]
		[Tooltip("The maximum duration between the start of two presses for them to be considered a double press.")]
		private float _doublePressDuration = 0.3f;
		/// <summary>
		/// The maximum duration between the start of two presses for them to be considered a double press.
		/// </summary>
		public static float doublePressDuration { 
			get => instance._doublePressDuration;
			set => instance._doublePressDuration = value;
		}

		[SerializeField]
		[Range(0,2)]
		[Tooltip("The minimum duration of a press for it to be considered a long press.")]
		private float _longPressDuration = 0.3f;
		/// <summary>
		/// The minimum duration of a press for it to be considered a long press.
		/// </summary>
		public static float longPressDuration { 
			get => instance._longPressDuration;
			set => instance._longPressDuration = value;
		}


		// --------------------
		// STICKS
		// --------------------

		public enum StickTypes { FourDirections = 90, EightDirections = 45 }
		[Header("Sticks")]

		[SerializeField]
		[Tooltip("The type of stick to use.\n\n"+
		         "- Set it to Four Directions for 4-directional sticks, with virtual buttons that span 1/4 of a " +
		         "circle (90 degrees). Use diagonals with caution in this case.\n\n" +
		         "- Set it to Eight Directions for 8-directional sticks, with virtual buttons that span 1/8 of a " +
		         "circle (45 degrees).")]
		private StickTypes _stickType = StickTypes.EightDirections;
		/// <summary>
		/// The type of stick to use. <br/> <br/>
		/// - Set it to Four Directions for 4-directional sticks, with virtual buttons that span 1/4 of a circle
		/// (90 degrees). Use diagonals with caution in this case.<br/>
		/// - Set it to Eight Directions for 8-directional sticks, with virtual buttons that span 1/8 of a circle
		/// (45 degrees).
		/// </summary>
		public static StickTypes stickType { 
			get => instance._stickType;
			set => instance._stickType = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the origin beyond which stick inputs start being registered.")]
		private float _stickDeadZone = 0.2f;
		/// <summary>
		/// The distance from the origin beyond which stick inputs start being registered.
		/// </summary>
		public static float stickDeadZone { 
			get => instance._stickDeadZone;
			set => instance._stickDeadZone = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the origin beyond which stick inputs are considered having a distance of 1.")]
		private float _stickMaxZone = 0.9f;
		/// <summary>
		/// The distance from the origin beyond which stick inputs are considered having a distance of 1.
		/// </summary>
		public static float stickMaxZone { 
			get => instance._stickMaxZone;
			set => instance._stickMaxZone = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the end of the dead zone beyond which stick inputs are considered pushed.")]
		private float _stickPressedZone = 0.5f;
		/// <summary>
		/// The distance from the end of the dead zone beyond which stick inputs are considered pushed.
		/// </summary>
		public static float stickPressedZone { 
			get => instance._stickPressedZone;
			set { instance._stickPressedZone = value; }  
		}

		[SerializeField]
		[Tooltip("The Camera on which the worldPositionCamera feature of Stick should be based. If no Camera is set, " +
		         "Hinput will try to find one on the scene.")]
		private Transform _worldCamera = null;
		/// <summary>
		/// The Camera on which the worldPositionCamera feature of Stick should be based. If no Camera is set,  
		/// Hinput will try to find one on the scene.
		/// </summary>
		public static Transform worldCamera { 
			get { 
				if (instance._worldCamera == null) {
					if (Camera.main != null) instance._worldCamera = Camera.main.transform;
					else if (FindObjectOfType<Camera>() != null) 
						instance._worldCamera = FindObjectOfType<Camera>().transform;
					else return null;
				}

				return instance._worldCamera;
			} 
			set => instance._worldCamera = value;
		}

		private static Camera _hinputCamera;
		public static Camera hinputCamera {
			get {
				if (_hinputCamera == null || _hinputCamera.transform != worldCamera) 
					_hinputCamera = worldCamera.GetComponent<Camera>();
				return _hinputCamera;
			}
		}


		// --------------------
		// TRIGGERS
		// --------------------

		[Header("Triggers")]

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the origin beyond which trigger inputs start being registered.")]
		private float _triggerDeadZone = 0.1f;
		/// <summary>
		/// The distance from the origin beyond which trigger inputs start being registered.
		/// </summary>
		public static float triggerDeadZone { 
			get => instance._triggerDeadZone;
			set => instance._triggerDeadZone = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the origin beyond which trigger inputs are considered having a position of 1.")]
		private float _triggerMaxZone = 1f;
		/// <summary>
		/// The distance from the origin beyond which trigger inputs are considered having a position of 1.
		/// </summary>
		public static float triggerMaxZone { 
			get => instance._triggerMaxZone;
			set => instance._triggerMaxZone = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The distance from the end of the dead zone beyond which trigger inputs are considered pushed.")]
		private float _triggerPressedZone = 0.5f;
		/// <summary>
		/// The distance from the end of the dead zone beyond which trigger inputs are considered pushed.
		/// </summary>
		public static float triggerPressedZone { 
			get => instance._triggerPressedZone;
			set => instance._triggerPressedZone = value;
		}


		// --------------------
		// VIBRATION DEFAULTS
		// --------------------

		[Header("Vibration Defaults")]

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The default intensity of the left (low-frequency) motor when controllers vibrate.\n\n"+
		         "The left motor's vibration feels like a low rumble.")]
		private float _vibrationDefaultLeftIntensity = 0.2f;
		/// <summary>
		/// The default intensity of the left (low-frequency) motor when controllers vibrate.<br/> <br/>
		/// The left motor's vibration feels like a low rumble.
		/// </summary>
		public static float vibrationDefaultLeftIntensity { 
			get => instance._vibrationDefaultLeftIntensity;
			set => instance._vibrationDefaultLeftIntensity = value;
		}

		[SerializeField]
		[Range(0,1)]
		[Tooltip("The default intensity of the right (high-frequency) motor when controllers vibrate.\n\n" +
		         "The right motor's vibration feels like a sharp buzz.")]
		private float _vibrationDefaultRightIntensity = 0.8f;
		/// <summary>
		/// The default intensity of the right (high-frequency) motor when controllers vibrate.<br/> <br/>
		/// The right motor's vibration feels like a sharp buzz.
		/// </summary>
		public static float vibrationDefaultRightIntensity { 
			get => instance._vibrationDefaultRightIntensity;
			set => instance._vibrationDefaultRightIntensity = value;
		}

		[SerializeField]
		[Range(0,2)]
		[Tooltip("The default duration of gamepad vibrations.")]
		private float _vibrationDefaultDuration = 0.2f;
		/// <summary>
		/// The default duration of gamepad vibrations.
		/// </summary>
		public static float vibrationDefaultDuration { 
			get => instance._vibrationDefaultDuration;
			set => instance._vibrationDefaultDuration = value;
		}
    }
}