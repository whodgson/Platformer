using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class MapInteractEventTrigger : MonoBehaviour
{
    private GameMasterController master;

    public GameObject event_source;
    private BoxCollider trigger;

    private bool is_triggered = false;

    void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();
        trigger = this.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!is_triggered)
            return;

        if (master.game_state != GameState.Game)
            return;

        if (!master.input_controller.Was_Input_Interact
            && master.input_controller.Is_Input_Interact)
            master.cutscene_controller.StartCutscene(event_source);

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == GameConstants.TAG_PLAYER)
        {
            if (!is_triggered)
            {
                is_triggered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            if (is_triggered)
            {
                is_triggered = false;
            }
        }
    }
}
