using System.Collections;
using UnityEngine;

namespace HinputClasses.Internal {
    // This class gathers some useful variables and methods.
	public static class Utils {
		// --------------------
		// STICKS
		// --------------------

		// Returns true if stick is currently within a (Settings.stickType) degree arc oriented at angle.
		public static bool PushedTowards (this Stick stick, float angle) => 
			(Mathf.Abs(Mathf.DeltaAngle(angle, stick.angle)) < ((float)Settings.stickType) / 2);


		// --------------------
		// COROUTINES
		// --------------------

		// A way of delegating StartCoroutine for classes that don't inherit MonoBehaviour.
		public static void Coroutine (IEnumerator coroutine) => Updater.instance.StartCoroutine(coroutine);

		// A way of delegating StopAllCoroutines for classes that don't inherit MonoBehaviour.
		public static void StopRoutines () => Updater.instance.StopAllCoroutines();


		// --------------------
		// MATH
		// --------------------

		private const float epsilon = 0.0000001f;

		public static bool IsEqualTo (this float target, float other) => Mathf.Abs(target - other) < epsilon;

		public static bool IsNotEqualTo (this float target, float other) => Mathf.Abs(target - other) > epsilon;

		public static Vector2 Clamp(this Vector2 target, float min, float max) => new Vector2(
				Mathf.Clamp(target.x, min, max),
				Mathf.Clamp(target.y, min, max));

		// Inverse lerp
		public static float Prel(this float target, float min, float max) => (target - min)/(max - min);


		// --------------------
		// LEGACY
		// --------------------
		
		public enum OS { Unknown, Windows, Mac, Linux }
		public static OS os => OS.Unknown;

		public static float GetAxis(string fullName, bool logError) => 0;
		public static bool GetButton(string fullName) => false;

		public static bool HinputIsInstalled() => true;
		public static string HinputInputArray() => "";
		public static string inputManagerPath = "";

		public static void SetupError() { }
		public static void UninstallError() { }
	}
}