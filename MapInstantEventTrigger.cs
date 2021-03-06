using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class MapInstantEventTrigger : MonoBehaviour
{
    private GameMasterController master;

    public GameObject event_source;
    private BoxCollider trigger;

    private bool is_triggered = false;
    public bool is_one_shot = false;

    private void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();
        trigger = this.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if(is_triggered && !is_one_shot)
        {
            // unset trigger if player leaves trigger bounds.

            if (!Physics.CheckBox(gameObject.transform.TransformPoint(trigger.center),
                trigger.bounds.size,
                this.transform.rotation,
                GameConstants.LAYER_MASK_ONLY_PLAYER))
                is_triggered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            if (!is_triggered)
            {
                is_triggered = true;
                master.cutscene_controller.StartCutscene(event_source);
            }
        }
    }
}
