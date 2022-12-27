using UnityEngine;
using VolumetricFogAndMist2;

namespace VolumetricFogAndMist2.Demos
{
    public class CapsuleController : MonoBehaviour
    {
        public VolumetricFog fogVolume;
        public float moveSpeed = 10f;
        public float fogHoleRadius = 8f;
        public float clearDuration = 0.2f;
        public float distanceCheck = 1f;

        private Vector3 lastPos = new Vector3(float.MaxValue, 0, 0);

        private void Update()
        {
            float disp = Time.deltaTime * moveSpeed;

            // moves capsule with arrow keys
            if (Hinput.keyboard.leftArrow.pressed)
            {
                transform.Translate(-disp, 0, 0);
            }
            else if (Hinput.keyboard.rightArrow.pressed)
            {
                transform.Translate(disp, 0, 0);
            }
            if (Hinput.keyboard.upArrow.pressed)
            {
                transform.Translate(0, 0, disp);
            }
            else if (Hinput.keyboard.downArrow.pressed)
            {
                transform.Translate(0, 0, -disp);
            }

            // do not call SetFogOfWarAlpha every frame; only when capsule moves
            if ((transform.position - lastPos).magnitude > distanceCheck)
            {
                lastPos = transform.position;
                fogVolume.SetFogOfWarAlpha(transform.position, fogHoleRadius, 0, clearDuration);
            }
        }
    }
}