using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class EventSetCameraController : MonoBehaviour, IEventController
{
    private GameMasterController master;
    public GameObject next_event_source = null;
    public GameConstants.CameraMode camera_mode;
    public CameraModeChangeData camera_mode_change_data;

    void Start()
    {
        master = GameMasterController.GetMasterController();
    }

    public GameObject GetNextEventSource()
    {
        return next_event_source;
    }

    public string GetEventType()
    {
        return GameConstants.EVENT_TYPE_SET_CAMERA;
    }

    public void StartEvent()
    {
        var player_camera_object = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);

        player_camera_object.GetComponent<CameraController>()
            .SetCamera(camera_mode, camera_mode_change_data);
    }

    public void ProcessEvent()
    {
        return;
    }

    public bool FinishEvent()
    {
        var player_camera_object = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);
        float fixed_transition = player_camera_object.GetComponent<CameraController>().Fixed_Transition;

        return fixed_transition >= 1.0f;
    }
}
