using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class EventSetPlayerAnimatorController : MonoBehaviour, IEventController
{
    private GameMasterController master;
    public GameObject next_event_source = null;

    public string trigger = string.Empty;

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
        return GameConstants.EVENT_TYPE_SET_PLAYER_ANIMATOR;
    }

    public void StartEvent()
    {
        var player = GameObject.Find(GameConstants.NAME_PLAYER);
        var animator = player.GetComponentInChildren<Animator>();
        animator.SetTrigger(trigger);
    }

    public void ProcessEvent()
    {
        return;
    }

    public bool FinishEvent()
    {
        return true;
    }
}
