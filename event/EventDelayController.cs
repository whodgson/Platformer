using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class EventDelayController : MonoBehaviour, IEventController
{
    private GameMasterController master;
    public GameObject next_event_source = null;

    private float start_time = 0.0f;
    public float delay_seconds = 0.0f;

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
        return GameConstants.EVENT_TYPE_DELAY;
    }

    public void StartEvent()
    {
        start_time = Time.time;
    }

    public void ProcessEvent()
    {
        return;
    }

    public bool FinishEvent()
    {
        return (Time.time - start_time) >= delay_seconds;
    }
}
