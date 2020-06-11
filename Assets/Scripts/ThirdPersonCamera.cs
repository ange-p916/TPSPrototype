using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    float distance;
    public LayerMask wall;

    Vector3 dollyDir;
    public float minDistance = 1f;
    public float maxDistance = 2f;

    float smooth = 10f;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public float dstFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public Transform camera_transform;

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    Vector3 new_cam_pos;
    public bool followOnStart = false;

    bool isFollowing = false;

    public float delta_rotation = 0f;

    float yaw;
    float pitch;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    [Range(0, 1)]
    public float airControlPercent;

    CharacterController controller;

    Vector3 cam_position;
    Vector3 cam_mask;
    Transform player_target;
    float nother_smooth;
    public float DistanceUp = -2;
    private float DistanceAway;

    public Vector3 offset_vector;

    private float HorizontalAxis;
    private float VerticalAxis;

    public void OnStartFollowing()
    {
        camera_transform = Camera.main.transform;
        isFollowing = true;

        DoCamStuff();
    }

    void Start()
    {
        if(followOnStart)
        {
            OnStartFollowing();
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        dollyDir = transform.position.normalized;
        distance = transform.position.magnitude;
    }


    void LateUpdate()
    {
        if(camera_transform == null && isFollowing)
        {
            OnStartFollowing();
        }
        
        if(isFollowing)
        {
            DoCamStuff();
        }
    }

    void MoreCamStuff()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        camera_transform.eulerAngles = currentRotation;

        camera_transform.position = this.transform.position - camera_transform.transform.forward * dstFromTarget + offset_vector;
    }

    public void DoCamStuff()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            float targetRotation = Mathf.Atan2(pitch, yaw) * Mathf.Rad2Deg + camera_transform.eulerAngles.y;

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            this.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(camera_transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            camera_transform.eulerAngles = currentRotation;

            delta_rotation = camera_transform.transform.eulerAngles.y;
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
   

            camera_transform.transform.eulerAngles = currentRotation;
        }


        RaycastHit hit;

        if (Physics.Linecast(this.transform.position, this.transform.position, out hit, wall))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            print("hitting: " + hit.distance);
        }
        else
        {
            distance = maxDistance;
        }
        camera_transform.position = this.transform.position - camera_transform.transform.forward * dstFromTarget + offset_vector;


    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (this.transform.GetComponent<CharacterController>().isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    void MyUpdate()
    {

        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");
        //horizontal and vert axis code
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        float targetRotation = Mathf.Atan2(this.transform.GetComponent<PlayerController>().inputDir.x, this.transform.GetComponent<PlayerController>().inputDir.y) * Mathf.Rad2Deg + camera_transform.transform.eulerAngles.y;

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        this.transform.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(camera_transform.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        camera_transform.eulerAngles = currentRotation;

        delta_rotation = camera_transform.transform.eulerAngles.y;

        Vector3 targetOffset = new Vector3(this.transform.position.x, (this.transform.position.y + 2f), this.transform.position.z);
        Vector3 rotateVector = Vector3.one;

        cam_position = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;
        cam_mask = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;

        OccludeRay(ref targetOffset);
        SmoothCamMethod();

        camera_transform.position = this.transform.position - camera_transform.transform.forward * dstFromTarget;
        DistanceAway = Mathf.Clamp(DistanceAway, minDistance, maxDistance);
    }

    void SmoothCamMethod()
    {
        nother_smooth = 4f;
        //TODO: change this transform from player's to camera's transform
        camera_transform.position = Vector3.Lerp(camera_transform.position, cam_position, Time.deltaTime * nother_smooth);
    }

    void OccludeRay(ref Vector3 Target_follow)
    {
        RaycastHit wall_hit = new RaycastHit();

        if(Physics.Linecast(Target_follow, cam_mask, out wall_hit, wall))
        {
            nother_smooth = 10f;
            camera_transform.position = new Vector3(wall_hit.point.x + wall_hit.normal.x * 0.5f, cam_position.y, wall_hit.point.z + wall_hit.normal.z * 0.5f);
        }
    }

}
