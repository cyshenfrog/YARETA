using DG.Tweening;
using System.Collections;
using UnityEngine;
using Cinemachine;

public enum CameraMode
{
    Default3rdPerson,
    OverShoulder,
    TopView,
    DragView,
    DefaultHigh,
}

public class CameraMain : UnitySingleton_D<CameraMain>
{
    public GameObject[] CameraGroup;
    public Transform Target;
    public Tool_TransformFollow Follower;

    //Invert input options;
    public bool invertHorizontalInput = false;

    public bool invertVerticalInput = false;

    private float autoRotateCD = 2;
    private bool autoRotate;

    //Current rotation values (in degrees);
    private float currentXAngle = 0f;

    private float currentYAngle = 0f;

    //Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the gameobject);
    [Range(0f, 90f)]
    public float upperVerticalLimit = 60f;

    [Range(0f, 90f)]
    public float lowerVerticalLimit = 60f;

    public Vector3 upperShif;
    public Vector3 lowerShif;
    public float upperRadiusOffset;
    public float lowerRadiusOffset;
    private Vector3 initTargetPos;

    //Variables to store old rotation values for interpolation purposes;
    private float oldHorizontalInput = 0f;

    private float oldVerticalInput = 0f;

    //Camera turning speed;
    public float cameraSpeed = 250f;

    //Whether camera rotation values will be smoothed;
    public bool smoothCameraRotation = false;

    //This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles;
    //Setting this value to '50f' (or above) will result in no smoothing at all;
    //Setting this value to '1f' (or below) will result in very noticable smoothing;
    //For most situations, a value of '25f' is recommended;
    [Range(1f, 50f)]
    public float cameraSmoothingFactor = 25f;

    //References to transform and camera components;
    public Transform CameraControl;

    private float _horizontalInput;
    private float _verticalInput;
    private float recenterCD = 5;
    public Transform RecenterTarget { get; set; }
    public bool softLock;
    public bool hardLock = true;
    private CameraMode cameraMode;

    //Setup references.
    public override void Awake()
    {
        base.Awake();
        initTargetPos = Target.localPosition;
        //Set angle variables to current rotation angles of this transform;
        currentXAngle = CameraControl.localRotation.eulerAngles.x > 180 ? CameraControl.localRotation.eulerAngles.x - 360 : CameraControl.localRotation.eulerAngles.x;

        currentYAngle = CameraControl.localRotation.eulerAngles.y;

        //Execute camera rotation code once to calculate facing and upwards direction;
        RotateCamera(0f, 0f);
    }

    public CinemachineVirtualCamera GetCurrentCam()
    {
        return CameraGroup[(int)cameraMode].GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void SetUpperShift(Vector3 value)
    {
        upperShif = value;
    }

    public void SetLowerShift(Vector3 value)
    {
        lowerShif = value;
    }

    public void SetVerticalLimit(float value, bool isTop)
    {
        if (isTop)
        {
            upperVerticalLimit = value;
        }
        else
        {
            lowerVerticalLimit = value;
        }
    }

    public void Recenter(float duration)
    {
        softLock = true;
        CameraControl.DOLookAt(Player.Instance.transform.position + Player.Instance.transform.forward * 10, duration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                currentXAngle = CameraControl.localRotation.eulerAngles.x > 180 ? CameraControl.localRotation.eulerAngles.x - 360 : CameraControl.localRotation.eulerAngles.x;
                currentYAngle = CameraControl.localRotation.eulerAngles.y;
            })
            .OnComplete(() =>
            {
                softLock = false;
            });
    }

    public void Recenter(float duration, float xAngle)
    {
        softLock = true;
        CameraControl.DORotate(Player.Instance.transform.eulerAngles + Vector3.right * xAngle, duration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                currentXAngle = CameraControl.localRotation.eulerAngles.x > 180 ? CameraControl.localRotation.eulerAngles.x - 360 : CameraControl.localRotation.eulerAngles.x;
                currentYAngle = CameraControl.localRotation.eulerAngles.y;
            })
            .OnComplete(() =>
            {
                softLock = false;
            });
    }

    public void LookAt(Transform Target, float duration)
    {
        softLock = true;
        CameraControl.DOLookAt(Target.position, duration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                currentXAngle = CameraControl.localRotation.eulerAngles.x > 180 ? CameraControl.localRotation.eulerAngles.x - 360 : CameraControl.localRotation.eulerAngles.x;
                currentYAngle = CameraControl.localRotation.eulerAngles.y;
            })

            .OnComplete(() =>
            {
                softLock = false;
            });
    }

    public void SetCameraMode(CameraMode mode)
    {
        cameraMode = mode;
        if (CameraGroup[(int)cameraMode].activeSelf)
            return;
        foreach (var cam in CameraGroup)
        {
            cam.SetActive(false);
        }
        CameraGroup[(int)cameraMode].SetActive(true);

        switch (cameraMode)
        {
            case CameraMode.Default3rdPerson:
                hardLock = false;
                break;

            case CameraMode.DefaultHigh:
                hardLock = false;
                break;

            case CameraMode.OverShoulder:
                hardLock = false;
                break;

            case CameraMode.TopView:
                hardLock = true;
                CameraGroup[(int)cameraMode].transform.DOLocalRotate(Vector3.right * (80 - CameraControl.transform.localEulerAngles.x), 0);
                break;

            case CameraMode.DragView:
                hardLock = true;
                break;

            default:
                break;
        }
    }

    public void TempLook(Transform target, float duration)
    {
        StartCoroutine(_TempLook(target, duration));
    }

    private IEnumerator _TempLook(Transform target, float duration)
    {
        Transform temp = Target;
        Target = target;
        yield return new WaitForSeconds(duration);
        Target = temp;
    }

    public void LockVirtical(float angle, bool snap = false, float duration = 1)
    {
        softLock = true;
        CameraControl.DOLocalRotate(new Vector3(angle, CameraControl.localEulerAngles.y), snap ? 0 : duration);
    }

    private void Update()
    {
        if (hardLock)
            return;
        if (currentXAngle > 0)
        {
            Target.localPosition = Vector3.Lerp(initTargetPos, initTargetPos + upperShif + Vector3.forward * upperRadiusOffset, currentXAngle / upperVerticalLimit);
        }
        else
        {
            Target.localPosition = Vector3.Lerp(initTargetPos, initTargetPos + lowerShif + Vector3.forward * lowerRadiusOffset, -currentXAngle / lowerVerticalLimit);
        }
        if (softLock)
        {
            if (!GameInput.IsCameraMove)
                return;
            else
            {
                softLock = false;
                CameraControl.DOKill();
            }
        }

        if (Hinput.anyGamepad.leftStick)
        {
            if (autoRotateCD > 0)
            {
                autoRotateCD -= Time.deltaTime;
                if (autoRotateCD < 0)
                    autoRotate = true;
            }
        }
        else if (Hinput.anyGamepad.rightStick)
        {
            autoRotateCD = 2;
            autoRotate = false;
        }

        if (GameInput.IsCameraMove)
        {
            recenterCD = 5;
            HandleCameraRotation();
            return;
        }

        //if (GameInput.IsMove)
        //{
        //    if (RecenterTarget)
        //    {
        //        if (recenterCD > 0)
        //        {
        //            recenterCD -= Time.deltaTime;
        //            if (recenterCD < 0)
        //            {
        //                LookAt(RecenterTarget, 5);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    recenterCD = 5;
        //}
    }

    public float GetHorizontalCameraInput()
    {
        //Get input;
        _horizontalInput = GameInput.CameraMove.x;
        if (autoRotate && Mathf.Abs(GameInput.Move.x) > 0.2f)
            _horizontalInput += GameInput.Move.x * 0.2f;
        //Handle inverted inputs;
        if (invertHorizontalInput)
            return _horizontalInput *= (-1f);
        else
            return _horizontalInput;
    }

    public float GetVerticalCameraInput()
    {
        //Get input;
        _verticalInput = GameInput.CameraMove.y / 2;

        //Handle inverted inputs;
        if (invertVerticalInput)
            return _verticalInput;
        else
            return _verticalInput *= (-1f);
    }

    //Get user input and handle camera rotation;
    //This method can be overridden in classes derived from this base class to modify camera behaviour;
    private void HandleCameraRotation()
    {
        //Get input values;
        float _inputHorizontal = GetHorizontalCameraInput();
        float _inputVertical = GetVerticalCameraInput();

        RotateCamera(_inputHorizontal, _inputVertical);
    }

    //Rotate camera;
    private void RotateCamera(float _newHorizontalInput, float _newVerticalInput)
    {
        if (smoothCameraRotation)
        {
            //Lerp input;
            oldHorizontalInput = Mathf.Lerp(oldHorizontalInput, _newHorizontalInput, Time.deltaTime * cameraSmoothingFactor);
            oldVerticalInput = Mathf.Lerp(oldVerticalInput, _newVerticalInput, Time.deltaTime * cameraSmoothingFactor);
        }
        else
        {
            //Replace old input directly;
            oldHorizontalInput = _newHorizontalInput;
            oldVerticalInput = _newVerticalInput;
        }

        //Add input to camera angles;
        currentXAngle += oldVerticalInput * cameraSpeed * Time.deltaTime;
        currentYAngle += oldHorizontalInput * cameraSpeed * Time.deltaTime;

        //Clamp vertical rotation;
        currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

        UpdateRotation();
    }

    //Update camera rotation based on x and y angles;
    private void UpdateRotation()
    {
        CameraControl.localRotation = Quaternion.Euler(new Vector3(currentXAngle, currentYAngle, 0));
    }
}