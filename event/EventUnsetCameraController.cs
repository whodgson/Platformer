using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class EventUnsetCameraController : MonoBehaviour, IEventController
{
    private GameMasterController master;
    public GameObject next_event_source = null;

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
        return GameConstants.EVENT_TYPE_UNSET_CAMERA;
    }

    public void StartEvent()
    {
        var player_camera_object = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);
        player_camera_object.GetComponent<CameraController>().UnsetCamera();
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
