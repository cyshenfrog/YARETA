using UnityEngine;

public class Camera_Collider : MonoBehaviour
{
    //Transform component of camera;
    public Transform cameraTransform;

    //Transform component of camera target;
    public Transform cameraTargetTransform;

    public void SetCameraTransform(Transform t)
    {
        cameraTransform = t;
    }

    public void SetCameraTargetTransform(Transform t)
    {
        cameraTargetTransform = t;
    }

    private Transform tr;

    //Whether a raycast or spherecast is used to scan for obstacles;
    public CastType castType;

    public enum CastType
    {
        Raycast,
        Spherecast
    }

    //Layermask used for raycasting;
    public LayerMask layerMask = ~0;

    private float currentDistance;

    //Additional distance which is added to the raycast's length to prevent the camera from clipping into level geometry;
    //For most situations, the default value of '0.1f' is sufficient;
    //You can try increasing this distance a bit if you notice a lot of clipping;
    //This value is only used if 'Raycast' is chosen as 'castType';
    public float minimumDistanceFromObstacles = 0.1f;

    //This value controls how smoothly the old camera distance will be interpolated toward the new distance;
    //Setting this value to '50f' (or above) will result in no (visible) smoothing at all;
    //Setting this value to '1f' (or below) will result in very noticable smoothing;
    //For most applications, a value of '25f' is recommended;
    public float smoothingFactor = 25f;

    //Radius of spherecast, only used if 'Spherecast' is chosen as 'castType';
    public float spherecastRadius = 0.2f;

    private void Awake()
    {
        tr = transform;

        if (cameraTransform == null)
            Debug.LogWarning("No camera transform has been assigned.", this);

        if (cameraTargetTransform == null)
            Debug.LogWarning("No camera target transform has been assigned.", this);

        //If the necessary transform references have not been assigned, disable this script;
        if (cameraTransform == null || cameraTargetTransform == null)
        {
            this.enabled = false;
            return;
        }

        //Set intial starting distance;
        currentDistance = (cameraTargetTransform.position - tr.position).magnitude;
    }

    private void LateUpdate()
    {
        //Calculate current distance by casting a raycast;
        float _distance = GetCameraDistance();

        //Lerp 'currentDistance' for a smoother transition;
        currentDistance = Mathf.Lerp(currentDistance, _distance, Time.deltaTime * smoothingFactor);

        //Set new position of 'cameraTransform';
        cameraTransform.position = tr.position + (cameraTargetTransform.position - tr.position).normalized * currentDistance;
    }

    //Calculate maximum distance by casting a ray (or sphere) from this transform to the camera target transform;
    private float GetCameraDistance()
    {
        RaycastHit _hit;

        //Calculate cast direction;
        Vector3 _castDirection = cameraTargetTransform.position - tr.position;

        if (castType == CastType.Raycast)
        {
            //Cast ray;
            if (Physics.Raycast(new Ray(tr.position, _castDirection), out _hit, _castDirection.magnitude + minimumDistanceFromObstacles, layerMask, QueryTriggerInteraction.Ignore))
            {
                //Check if 'minimumDistanceFromObstacles' can be subtracted from '_hit.distance', then return distance;
                if (_hit.distance - minimumDistanceFromObstacles < 0f)
                    return _hit.distance;
                else
                    return _hit.distance - minimumDistanceFromObstacles;
            }
        }
        else
        {
            //Cast sphere;
            if (Physics.SphereCast(new Ray(tr.position, _castDirection), spherecastRadius, out _hit, _castDirection.magnitude, layerMask, QueryTriggerInteraction.Ignore))
            {
                //Return distance;
                return _hit.distance;
            }
        }
        //If no obstacle was hit, return full distance;
        return _castDirection.magnitude;
    }
}