using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputController : MonoBehaviour
{
    public InputActionAsset controls;
    InputActionMap action_map;

    public InputAction action_positive;
    public InputAction action_negative;
    public InputAction action_horizontal;
    public InputAction action_vertical;
    public InputAction action_interact;
    public InputAction action_inspect;
    public InputAction action_start;
    public InputAction action_select;
    public InputAction action_aim_horizontal;
    public InputAction action_aim_vertical;
    public InputAction action_aim_zoom;

    public float sensitivity_camera_zoom = 0.05f;
    public float sensitivity_camera_horizontal = 0.7f;
    public float sensitivity_camera_vertical = 0.7f;

    public bool was_input_positive = false;
    public bool was_input_negative = false;

    public bool is_input_positive = false;
    public bool is_input_negative = false;

    void Awake()
    {
        action_map = controls.FindActionMap("action_map");

        action_positive = action_map.FindAction("positive");
        action_negative = action_map.FindAction("negative");
        action_horizontal = action_map.FindAction("horizontal");
        action_vertical = action_map.FindAction("vertical");
        action_interact = action_map.FindAction("interact");
        action_inspect = action_map.FindAction("inspect");
        action_start = action_map.FindAction("start");
        action_select = action_map.FindAction("select");
        action_aim_horizontal = action_map.FindAction("aim_horizontal");
        action_aim_vertical = action_map.FindAction("aim_vertical");
        action_aim_zoom = action_map.FindAction("aim_zoom");

        action_positive.Enable();
        action_negative.Enable();
        action_horizontal.Enable();
        action_vertical.Enable();
        action_interact.Enable();
        action_inspect.Enable();
        action_start.Enable();
        action_select.Enable();
        action_aim_horizontal.Enable();
        action_aim_vertical.Enable();
        action_aim_zoom.Enable();
    }

    void Update()
    {
        was_input_positive = is_input_positive;
        was_input_negative = is_input_negative;

        is_input_positive = action_positive.ReadValue<float>() >= 0.1f;
        is_input_negative = action_negative.ReadValue<float>() >= 0.1f;
    }
}
