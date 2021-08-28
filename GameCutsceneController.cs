using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using TMPro;

public class GameCutsceneController : MonoBehaviour
{
    const float EVENT_STEP_INTERVAL_DEFAULT = 0.05f;
    const float EVENT_STEP_INTERVAL_FAST = 0.025f;

    public GameMasterController master;
    public GameUserInterfaceController ui;

    public GameObject event_source;
    public IEventController current_event;

    float current_event_time = 0f;

    float current_event_step_time = 0f;
    float current_event_step_time_interval = 0.05f;

    bool is_current_event_item_started = false;
    bool is_current_event_item_finished = false;

    // text variables.

    public string message_box_text = string.Empty;

    void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();
        ui = master.user_interface_controller;
    }

    void Update()
    {
        if (master.game_state != GameState.Cutscene)
            return;

        // start event.
        if (!is_current_event_item_started)
            StartEventItem();

        current_event_time += Time.deltaTime;
        current_event_step_time += Time.deltaTime;

        current_event_step_time_interval = (master.input_controller.is_input_positive)
            ? EVENT_STEP_INTERVAL_FAST
            : EVENT_STEP_INTERVAL_DEFAULT;

        if (current_event_step_time >= current_event_step_time_interval)
        {
            current_event_step_time = 0.0f;

            // continue event.
            ProcessEventItem();
        }

        // finish event if possible.
        FinishEventItem();

        if(is_current_event_item_finished)
        {
            is_current_event_item_started = false;
            is_current_event_item_finished = false;

            // move onto next event, or finish
            // when meeting right criteria.

            if (current_event.GetNextEventSource() == null)
            {
                // return to game if there are no more items.
                EndCutscene();
            }
            else
            {
                // start the next cutscene event.
                StartCutscene(current_event.GetNextEventSource());
            }
        }
    }

    private void StartEventItem()
    {
        current_event_time = 0.0f;
        current_event_step_time = 0.0f;

        is_current_event_item_started = true;

        current_event.StartEvent();
    }

    private void ProcessEventItem()
    {
        // handle the current event item.

        current_event.ProcessEvent();
    }
    
    private void FinishEventItem()
    {
        if (current_event == null)
            return;

        is_current_event_item_finished = current_event.FinishEvent();
    }

    public void StartCutscene(GameObject event_source)
    {
        master.ChangeState(GameState.Cutscene);

        this.event_source = event_source;
        this.current_event = event_source.GetComponent<IEventController>();

        current_event_time = 0f;
        current_event_step_time = 0f;
    }

    public void EndCutscene()
    {
        master.ChangeState(GameState.Game);
    }
}
