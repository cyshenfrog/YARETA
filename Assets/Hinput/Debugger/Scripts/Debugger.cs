using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HinputClasses.Internal {
	// Hinput class testing every feature of the plugin and logging them to the console.
    public class Debugger : MonoBehaviour {
		// --------------------
		// SETTINGS
		// --------------------

		[Header("GENERAL")]
		public bool startMessage;
		public bool lockCurrentInput;

		public enum BF { none, defaultCast, simplePress, doublePress, longPress, triggerPosition }
		public enum PF { defaultCast,  justPressedAndJustReleased, pressedAndReleased, pressDurationAndReleaseDuration }
		[Header("BUTTONS")]
		public BF buttonFeature;
		public PF pressFeature;

		public enum SD { none, verticalsAndHorizontals, diagonals, pressedZone }
		public enum SF { 
			none, position, horizontal, vertical, angle, distance, worldPositionCamera, worldPositionFlat 
		}
		[Header("GAMEPADS")]
		public SD stickDirectionsAsButtons;
		public SF stickFeature;
		public bool vibrateOnVPressed;
		
		public enum MF { 
			none, position, pixelPosition, delta, pixelDelta, scrollPosition, scrollUpAndDown, hoverFirst, hoverAll, 
			clickFirst, clickAll 
		}
		[Header("MOUSE")]
		public MF mouseFeature;

		[Header("TIME")]
		[Space(20)]
		[Header("--------------------")]
		[Space(20)]
		
		[Range(0,3)]
		public float timeScale = 1;
		public bool playInUpdate;
		public bool playInFixedUpdate;

		public enum GM { individualGamepads, anyGamepad }
		public enum IM { individualInputs, anyInput }
		[Header("GAMEPAD AND INPUT MODE")] 
		public GM gamepadMode;
		public IM inputMode;

		[Header("INFO")] 
		public bool gamepadInfoOnGPressed;
		public bool gamepadListsOnLPressed;
		public bool stickInfoOnPPressed;
		public bool buttonInfoOnBPressed;

		[Header("ENABLE/DISABLE")] 
		public bool toggleGamepadOnGPressed;
		public bool toggleStickOnPPressed;
		public bool toggleButtonOnBPressed;

		public enum VM {
			noArgs, duration, intensity, durationAndIntensity, oneCurve, twoCurves, vibrationPreset, advanced
		}
		[Header("VIBRATION")]
		[Range(-1,8)] public int gamepadToVibrate;
		public VM vibrationMode;
		[Range(0,10)]
		public float duration;
		[Range(0,1)]
		public float leftIntensity;
		[Range(0,1)]
		public float rightIntensity;
		public AnimationCurve curve;
		public AnimationCurve leftCurve;
		public AnimationCurve rightCurve;
		public VibrationPreset vibrationPreset;
		public bool multiplyVibrationPreset;
		public bool stopVibrationOnSPressed;
		[Range(-1, 3)]
		public float stopVibrationDuration;
		public bool displayIntensity;

		[Header("REFERENCES")]
		[Space(20)]
		[Header("--------------------")]
		[Space(20)]

		public GameObject message;
		public Transform plane;
		public Transform redCube;
		public Transform blueSphere;
		public float moveSpeed;

		private Stick currentStick;
		private Pressable currentButton;


		// --------------------
		// START AND UPDATE
		// --------------------


		private void Start () {
			if (startMessage) Debug.Log("Press a button to log it in the debugger");
		}

		private void Update () {
			Time.timeScale = timeScale;
			if (playInUpdate) TestEverything();
		}

		private void FixedUpdate () {
			if (playInFixedUpdate) TestEverything();
		}

		private void TestEverything() {
			TestInfo();
			TestEnableDisable();
			TestSticks ();
			TestButtons ();
			TestVibration ();
			TestMouse();
		}


		// --------------------
		// TEST GAMEPADS
		// --------------------

		private void TestInfo() {
			if (gamepadInfoOnGPressed && Hinput.keyboard.G.justPressed) {
				Gamepad currentGamepad = (currentButton as GamepadPressable)?.gamepad;
				if (currentGamepad == null) {
					Debug.Log("Current gamepad has not been set");
					return;
				}
				Debug.Log("current gamepad: " +
				          "[isConnected = " + currentGamepad.isConnected +
				          ", isEnabled = " + currentGamepad.isEnabled +
				          ", type = " + currentGamepad.type + 
				          ", index = " + currentGamepad.index + 
				          ", name = " + currentGamepad.name+ "]");
			}

			if (gamepadListsOnLPressed && Hinput.keyboard.L.justPressed) {
				if (currentButton == null) {
					Debug.Log("Current gamepad has not been set");
					return;
				}
				
				Gamepad currentGamepad = (currentButton as GamepadPressable)?.gamepad;
				if (currentGamepad == null) {
					Debug.Log("Current gamepad has not been set");
					return;
				}
				Debug.Log("current gamepad buttons : " + 
				          ToString(currentGamepad.buttons.Select(button => button.name).ToList()) +
				          ", current gamepad sticks : " + 
				          ToString(currentGamepad.sticks.Select(stick => stick.name).ToList()));
			}
			
			if (stickInfoOnPPressed && Hinput.keyboard.P.justPressed) {
				if (currentStick == null) {
					Debug.Log("Current stick has not been set");
					return;
				}

				Debug.Log("current stick: [isEnabled = " + currentStick.isEnabled +
				          ", index = " + currentStick.index +
				          ", name = " + currentStick.name +
				          ", gamepad = " + currentStick.gamepad.name + "]");
			}
			
			if (buttonInfoOnBPressed && Hinput.keyboard.B.justPressed) {
				if (currentButton == null) {
					Debug.Log("Current button has not been set");
					return;
				}
				
				string log = "isEnabled = " + currentButton.isEnabled + 
				             ", name = " + currentButton.name +
				             ", gamepad = " + (currentButton as GamepadPressable)?.gamepad?.name;
				
				if (currentButton is StickDirection) {
					log = "current direction: [" + log +
					      ", stick = " + ((StickDirection)currentButton).stick.name;
				} else if (currentButton is StickPressedZone) {
					log = "current stick pressed zone: [" + log +
					      ", stick = " + ((StickPressedZone)currentButton).stick.name;
				} else if (currentButton is Button) {
					log = "current button: [" + log +
					      ", index = " + ((Button)currentButton).index;
				} else if (currentButton is Trigger) {
					log = "current trigger: [" + log +
					      ", index = " + ((Trigger)currentButton).index;
				} else if (currentButton is AnyInput) {
					log = "current anyInput: [" + log;
				}

				log += "]";
				Debug.Log(log);
			}
		}


		// --------------------
		// TEST ENABLE/DISABLE
		// --------------------

		private void TestEnableDisable() {
			if (toggleGamepadOnGPressed && Hinput.keyboard.G.justPressed) {
				if (!(currentButton is GamepadPressable)) {
					Debug.Log("Current gamepad has not been set");
					return;
				}
				if (((GamepadPressable) currentButton).gamepad.isEnabled) {
					(currentButton as GamepadPressable)?.gamepad.Disable();
					Debug.Log("Disabling " + (currentButton as GamepadPressable)?.gamepad.name);
				} else {
					(currentButton as GamepadPressable)?.gamepad.Enable();
					Debug.Log("Enabling "+ (currentButton as GamepadPressable)?.gamepad.name);
				}
			}
			
			if (toggleStickOnPPressed && Hinput.keyboard.P.justPressed) {
				if (currentStick == null) {
					Debug.Log("Current stick has not been set");
					return;
				}
				if (currentStick.isEnabled) {
					currentStick.Disable();
					Debug.Log("Disabling " + currentStick.name);
				} else {
					currentStick.Enable();
					Debug.Log("Enabling " + currentStick.name);
				}
			}
			
			if (toggleButtonOnBPressed && Hinput.keyboard.P.justPressed) {
				if (currentButton == null) {
					Debug.Log("Current button has not been set");
					return;
				}
				if (currentButton.isEnabled) {
					currentButton.Disable();
					Debug.Log("Disabling " + currentButton.name);
				} else {
					currentButton.Enable();
					Debug.Log("Enabling " + currentButton.name);
				}
			}
		}


		// --------------------
		// TEST BUTTONS
		// --------------------

		private void TestButtons () {
			if (!lockCurrentInput && (currentButton == null || currentButton.released)) 
				currentButton = GetNewCurrentButton();
			if (currentButton != null) TestCurrentButton ();
		}

		private Pressable GetNewCurrentButton () {
			List<Gamepad> gamepadsToTest;
			if (gamepadMode == GM.individualGamepads) gamepadsToTest = Hinput.gamepad;
			else gamepadsToTest = new List<Gamepad> {Hinput.anyGamepad};

			if (Hinput.anyGamepad.anyInput) {
				if (inputMode == IM.individualInputs) 
					return AllGamepadButtons(gamepadsToTest.FirstOrDefault(gamepad => gamepad.anyInput))
						.FirstOrDefault(button => button.pressed);
				else return gamepadsToTest.FirstOrDefault(gamepad => gamepad.anyInput)?.anyInput;
			}

			if (Hinput.mouse.leftClick) return Hinput.mouse.leftClick;
			if (Hinput.mouse.rightClick) return Hinput.mouse.rightClick;
			if (Hinput.mouse.middleClick) return Hinput.mouse.middleClick;

			if (Hinput.keyboard.currentInput) return Hinput.keyboard.currentInput;

			return currentButton;
		}


		// --------------------
		// GET ALL GAMEPAD BUTTONS
		// --------------------

		private List<Pressable> AllGamepadButtons (Gamepad gamepad) {
			List<Pressable> buttons = new List<Pressable>();
			
			buttons.AddRange (new List<Pressable>() {
				gamepad.A, gamepad.B, gamepad.X, gamepad.Y, gamepad.leftBumper, gamepad.rightBumper, 
				gamepad.leftTrigger, gamepad.rightTrigger, gamepad.leftStickClick, gamepad.rightStickClick, 
				gamepad.back, gamepad.start
			});

			if (stickDirectionsAsButtons == SD.verticalsAndHorizontals) buttons.AddRange (new List<Pressable> {
				gamepad.leftStick.up, gamepad.leftStick.down, gamepad.leftStick.left, gamepad.leftStick.right,
				gamepad.rightStick.up, gamepad.rightStick.down, gamepad.rightStick.left, gamepad.rightStick.right,
				gamepad.dPad.up, gamepad.dPad.down, gamepad.dPad.left, gamepad.dPad.right
			});

			if (stickDirectionsAsButtons == SD.diagonals) buttons.AddRange (new List<Pressable> {
				gamepad.leftStick.upLeft, gamepad.leftStick.upRight, gamepad.leftStick.downLeft, gamepad.leftStick.downRight,
				gamepad.rightStick.upLeft, gamepad.rightStick.upRight, gamepad.rightStick.downLeft, gamepad.rightStick.downRight,
				gamepad.dPad.upLeft, gamepad.dPad.upRight, gamepad.dPad.downLeft, gamepad.dPad.downRight
			});

			if (stickDirectionsAsButtons == SD.pressedZone) buttons.AddRange (new List<Pressable> {
				gamepad.leftStick.inPressedZone, gamepad.rightStick.inPressedZone, gamepad.dPad.inPressedZone
			});

			return buttons;
		}


		// --------------------
		// TEST CURRENT BUTTON
		// --------------------

		private void TestCurrentButton () {
			if (buttonFeature == BF.none) return;
			if (buttonFeature == BF.triggerPosition) {
				if (currentButton is Trigger) {
					Debug.Log (currentButton.name +  " position : " + ((Trigger)currentButton).position);
				} else Debug.Log("Pressable position is only available on triggers!");
			}
			
			Press currentPress;
			string adjective;
			if (buttonFeature == BF.defaultCast) {
				currentPress = currentButton;
				adjective = "default ";
			} else if (buttonFeature == BF.simplePress) {
				currentPress = currentButton.simplePress;
				adjective = "";
			} else if (buttonFeature == BF.doublePress) {
				currentPress = currentButton.doublePress;
				adjective = "double ";
			} else if (buttonFeature == BF.longPress) {
				currentPress = currentButton.longPress;
				adjective = "long ";
			} else return;

			if (pressFeature == PF.defaultCast) {
				if (currentPress) Debug.Log(currentButton.name + " is being " + adjective + "default pressed!!");
				else Debug.Log(currentButton.name + " is not being " + adjective + "default pressed");
			} else if (pressFeature == PF.pressedAndReleased) {
				if (currentPress.pressed) Debug.Log(currentButton.name + " is being " + adjective + "pressed!!");
				else Debug.Log(currentButton.name + " is not being " + adjective + "pressed");
			} else if (pressFeature == PF.justPressedAndJustReleased) {
				if (currentPress.justPressed) 
					Debug.Log(currentButton.name + " was just " + adjective + "pressed!!");
				if (currentPress.justReleased) 
					Debug.Log(currentButton.name + " was just released after a " + adjective + "press");
			} else if (pressFeature == PF.pressDurationAndReleaseDuration) {
				if (currentPress.pressed) 
					Debug.Log (currentButton.name + " has been held (" + adjective + "press) for " + 
					           currentPress.pressDuration+" seconds!!!");
				else Debug.Log (currentButton.name + " has been released (" + adjective + "press) for " + 
				                currentPress.releaseDuration + " seconds");
			}
		}


		// --------------------
		// TEST STICKS
		// --------------------

		private void TestSticks () {
			if (!lockCurrentInput && (currentStick == null || !currentStick.inPressedZone)) GetNewCurrentStick ();
			if (currentStick != null) TestCurrentStick ();
		}

		private void GetNewCurrentStick () {
			if (gamepadMode == GM.individualGamepads) Hinput.gamepad.ForEach(UpdateCurrentStickFromGamepad);
			else UpdateCurrentStickFromGamepad(Hinput.anyGamepad);
		}

		private void UpdateCurrentStickFromGamepad (Gamepad gamepad) {
			if (gamepad.leftStick.inPressedZone) currentStick = gamepad.leftStick;
			else if (gamepad.rightStick.inPressedZone) currentStick = gamepad.rightStick;
			else if (gamepad.dPad.inPressedZone) currentStick = gamepad.dPad;
		}


		// --------------------
		// TEST CURRENT STICK
		// --------------------

		private void TestCurrentStick () {
			if (stickFeature == SF.worldPositionCamera) {
				message.gameObject.SetActive(false);
				plane.gameObject.SetActive(true);
				redCube.gameObject.SetActive(false);
				blueSphere.gameObject.SetActive(true);
				Debug.Log (currentStick.name+" is controlling the blue sphere");
				blueSphere.transform.position += moveSpeed * Time.deltaTime * currentStick.worldPositionCamera;
			} else if (stickFeature == SF.worldPositionFlat) {
				message.gameObject.SetActive(false);
				plane.gameObject.SetActive(true);
				redCube.gameObject.SetActive(true);
				blueSphere.gameObject.SetActive(false);
				Debug.Log (currentStick.name+" is controlling the red cube");
				redCube.transform.position += moveSpeed * Time.deltaTime * currentStick.worldPositionFlat;
			} else {
				message.gameObject.SetActive(true);
				plane.gameObject.SetActive(false);
				redCube.gameObject.SetActive(false);
				blueSphere.gameObject.SetActive(false);
			}
			if (stickFeature == SF.none) return;
			if (stickFeature == SF.position) 
				Debug.Log (currentStick.name+" position : "+currentStick.position);
			if (stickFeature == SF.horizontal) Debug.Log (currentStick.name+
			                                              " horizontal : "+currentStick.horizontal);
			if (stickFeature == SF.vertical) Debug.Log (currentStick.name+
			                                            " vertical : "+currentStick.vertical);
			if (stickFeature == SF.angle) Debug.Log (currentStick.name+
			                                         " angle : "+currentStick.angle);
			if (stickFeature == SF.distance) Debug.Log (currentStick.name+
			                                            " distance : "+currentStick.distance);
		}

		// --------------------
		// TEST VIBRATION
		// --------------------

		private void TestVibration () {
			if (gamepadMode == GM.anyGamepad) TestVibrationOnGamepad(Hinput.anyGamepad);
			if (gamepadMode == GM.individualGamepads) {
				if (gamepadToVibrate == -1) TestVibrationOnGamepad(Hinput.anyGamepad);
				else TestVibrationOnGamepad(Hinput.gamepad[gamepadToVibrate]);
			}
		}

		private void TestVibrationOnGamepad(Gamepad gamepad) {
			if (vibrateOnVPressed && Hinput.keyboard.V.justPressed) {
				if (vibrationMode == VM.noArgs) gamepad.Vibrate();
				if (vibrationMode == VM.duration) gamepad.Vibrate(duration);
				if (vibrationMode == VM.intensity) gamepad.Vibrate(leftIntensity, rightIntensity);
				if (vibrationMode == VM.durationAndIntensity) gamepad.Vibrate(leftIntensity, rightIntensity, duration);
				if (vibrationMode == VM.oneCurve) gamepad.Vibrate(curve);
				if (vibrationMode == VM.twoCurves) gamepad.Vibrate(leftCurve, rightCurve);
				if (vibrationMode == VM.advanced) gamepad.VibrateAdvanced(leftIntensity, rightIntensity);
				if (vibrationMode == VM.vibrationPreset) {
					if (multiplyVibrationPreset) gamepad.Vibrate(vibrationPreset, leftIntensity, rightIntensity, duration);
					else gamepad.Vibrate(vibrationPreset);
				}
			}

			if (stopVibrationOnSPressed && Hinput.keyboard.S.justPressed) {
				if (stopVibrationDuration.IsEqualTo(0)) gamepad.StopVibration();
				else gamepad.StopVibration(stopVibrationDuration);
			}

			if (displayIntensity) {
				Debug.Log("left vibration : " + gamepad.leftVibration + ", right vibration : "
				          + gamepad.rightVibration);
			}
		}

		// --------------------
		// TEST MOUSE
		// --------------------

		private void TestMouse() {
			if (mouseFeature == MF.position) Debug.Log("Mouse position : "+Hinput.mouse.position);
			if (mouseFeature == MF.pixelPosition) Debug.Log("Mouse position (pixels) : "+Hinput.mouse.pixelPosition);
			if (mouseFeature == MF.delta) Debug.Log("Mouse delta : "+Hinput.mouse.delta);
			if (mouseFeature == MF.pixelDelta) Debug.Log("Mouse delta (pixels) : "+Hinput.mouse.pixelDelta);

			if (mouseFeature == MF.scrollPosition) Debug.Log("Scroll position : " + Hinput.mouse.scroll.position);
			if (mouseFeature == MF.scrollUpAndDown) {
				if (Hinput.mouse.scroll.up) Debug.Log("Scrolling up !!!");
				if (Hinput.mouse.scroll.down) Debug.Log("Scrolling down !!!");
			}

			if (mouseFeature == MF.hoverFirst || mouseFeature == MF.hoverAll || 
			    mouseFeature == MF.clickFirst || mouseFeature == MF.clickAll) {
				message.gameObject.SetActive(false);
				plane.gameObject.SetActive(true);
				redCube.gameObject.SetActive(true);
				blueSphere.gameObject.SetActive(true);
				if ((mouseFeature == MF.clickFirst && Hinput.mouse.Clicked(plane.gameObject)) ||
				    (mouseFeature == MF.clickAll && Hinput.mouse.Clicked(plane.gameObject, false)))
					Debug.Log("plane clicked !!!");
				if ((mouseFeature == MF.hoverFirst && Hinput.mouse.Over(plane.gameObject)) ||
				    (mouseFeature == MF.hoverAll && Hinput.mouse.Over(plane.gameObject, false)))
					Debug.Log("plane is being hovered");
			} else {
				message.gameObject.SetActive(true);
				plane.gameObject.SetActive(false);
				redCube.gameObject.SetActive(false);
				blueSphere.gameObject.SetActive(false);
			}
		}

		// --------------------
		// UTILS
		// --------------------

		private static string ToString<T>(List<T> list) {
			if (list.Count == 0) return "[]";
			string result = "[";
			for (int i = 0; i < list.Count - 1; i++) result += list[i] + ", ";
			return result + list[list.Count - 1] + "]";
		}

		private static string ToString(List<GameObject> list) {
			if (list.Count == 0) return "[]";
			string result = "[";
			for (int i = 0; i < list.Count - 1; i++) result += list[i].name + ", ";
			return result + list[list.Count - 1].name + "]";
		}
	}
}