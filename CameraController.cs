using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class CameraController : MonoBehaviour
{
    const float Y_MIN = -20;
    const float Y_MAX =  60;

    const float X_SPEED = 1f;
    const float Y_SPEED = 1f;
    const float ZOOM_SPEED = 1f;

    const float CAMERA_CLIPPING_RADIUS = 0.05f;

    const float MAX_DISTANCE_MIN = 1.0f;
    const float MAX_DISTANCE_MAX = 5.0f;
    
    public Transform target;
    public float target_distance;
    public float distance;

    private float x = 0.0f;
    private float y = 0.0f;

    // Fixed camera variables.

    private bool is_fixed = false;
    private Transform fixed_transform = null;
    private float fixed_transition = 0f;

    private Vector3 fixed_start_position;
    private Quaternion fixed_start_rotation;

    const float FIXED_TRANSITION_STEP = 2f;

    // game variables.

    GameMasterController master;

    void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();

        fixed_start_position = this.transform.position;
        fixed_start_rotation = this.transform.rotation;

        fixed_transform = this.transform;
        fixed_transition = 1.0f;

        // set to default target (player).

        target = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER_CAMERA_TARGET).transform;
    }

    void Update()
    {
        if (is_fixed)
            UpdateCameraFixed();
        else
            UpdateCameraNotFixed();
    }

    private void UpdateCameraFixed()
    {
        // Get the transition speed.

        float tran_speed = FIXED_TRANSITION_STEP * Time.deltaTime;

        // Increment the transition amount (0 is none, 1 is complete).

        fixed_transition += tran_speed;
        fixed_transition = Mathf.Clamp(fixed_transition, 0, 1);

        // Lerp between the start and end points.

        transform.position = Vector3.Lerp(fixed_start_position, fixed_transform.position, fixed_transition);
        transform.rotation = Quaternion.Lerp(fixed_start_rotation, fixed_transform.rotation, fixed_transition);

        // Set the X and Y rotation to the current rotation.
        // So the dynamic camera is in the same position when
        // exiting the fixed camera zone.

        x = transform.rotation.eulerAngles.y;
        y = transform.rotation.eulerAngles.x;
    }

    private void UpdateCameraNotFixed()
    {
        // Get x and y offset from input.

        x += master.input_controller.action_aim_horizontal.ReadValue<float>() * (X_SPEED 
            * master.input_controller.sensitivity_camera_horizontal);

        y -= master.input_controller.action_aim_vertical.ReadValue<float>() * (Y_SPEED 
            * master.input_controller.sensitivity_camera_vertical);

        // Get max distance from input.

        target_distance += master.input_controller.action_aim_zoom.ReadValue<float>() 
            * (ZOOM_SPEED * master.input_controller.sensitivity_camera_zoom);

        if (target_distance > MAX_DISTANCE_MAX) target_distance = MAX_DISTANCE_MAX;
        if (target_distance < MAX_DISTANCE_MIN) target_distance = MAX_DISTANCE_MIN;

        // Limit how low or high y can go.

        y = Mathf.Clamp(y, Y_MIN, Y_MAX);

        // Get the rotation from x and y.

        var rotation = Quaternion.Euler(y, x, 0.0f);

        // Get the camera's distance, checking for collision.

        RaycastHit hit;
        if (Physics.SphereCast(target.position, CAMERA_CLIPPING_RADIUS, transform.forward * -1, out hit, target_distance))
        {
            distance = hit.distance;
        }
        else
        {
            if (distance < target_distance)
            {
                distance += 0.1f;
            }
            if (distance > target_distance)
            {
                distance = target_distance;
            }
        }

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        // Apply the position and rotation to the camera.

        if(fixed_transition < 1.0f)
        {
            // Get the transition speed.

            float tran_speed = FIXED_TRANSITION_STEP * Time.deltaTime;

            // Increment the transition amount (0 is none, 1 is complete).

            fixed_transition += tran_speed;
            fixed_transition = Mathf.Clamp(fixed_transition, 0, 1);

            // Lerp between the start and end points.

            transform.position = Vector3.Lerp(fixed_start_position, position, fixed_transition);
            transform.rotation = Quaternion.Lerp(fixed_start_rotation, rotation, fixed_transition);
        }
        else
        {
            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public void SetFixedCamera(Transform fixed_transform)
    {
        this.is_fixed = true;

        // Initiate the transition to a fixed camera angle.

        fixed_start_position = this.transform.position;
        fixed_start_rotation = this.transform.rotation;
        fixed_transition = 0.0f;

        this.fixed_transform = fixed_transform;
    }

    public void UnsetFixedCamera()
    {
        this.is_fixed = false;

        // Reset the transition.

        fixed_start_position = this.transform.position;
        fixed_start_rotation = this.transform.rotation;
        fixed_transition = 0.0f;
    }
}
