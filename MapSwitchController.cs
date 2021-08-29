using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class MapSwitchController : MonoBehaviour
{
    const float PRESSED_TIMER_MULTIPLIER = 2;

    public string switch_game_var = null;
    public GameObject switch_plate;
    public GameObject switch_plate_start_position;
    public GameObject switch_plate_end_position;
    public bool is_permanent;
    public AudioClip switch_sound;
    public List<MapTriggerReceiverController> switch_receivers;

    private bool is_pressed = false;
    private AudioSource switch_audio_source;
    private float pressed_timer = 0.0f;

    private GameMasterController master_controller;

    // Start is called before the first frame update
    void Start()
    {
        master_controller = GameMasterController.GetMasterController();

        switch_audio_source = gameObject.AddComponent<AudioSource>();
        switch_audio_source.clip = switch_sound;
        switch_audio_source.loop = false;
        switch_audio_source.playOnAwake = false;
        switch_audio_source.volume = 1f * master_controller.audio_controller.volume_object;

        if(switch_game_var != null && switch_game_var != string.Empty)
        {
            is_pressed = master_controller.data_controller.GetGameVarBool(switch_game_var);

            if (is_pressed)
            {
                foreach (var switch_receiver in switch_receivers)
                {
                    switch_receiver.OnActivate();
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(is_pressed)
        {
            if(pressed_timer < 1)
                pressed_timer += Time.deltaTime * PRESSED_TIMER_MULTIPLIER;
        }
        else
        {
            if (pressed_timer > 0)
                pressed_timer -= Time.deltaTime * PRESSED_TIMER_MULTIPLIER;
        }

        // if the timer is not resting, LERP the switch
        // position between its start and end points.

        if (pressed_timer > 0 && pressed_timer < 1)
        {
            switch_plate.transform.position = Vector3.Lerp
                (switch_plate_start_position.transform.position,
                switch_plate_end_position.transform.position,
                pressed_timer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // no action if switch is already pressed.
        if (is_pressed)
            return;

        Debug.Log("Switch Trigger");

        var other_tags = other.gameObject.GetComponent<ActorTagController>();
        if (other_tags == null)
            return;

        if(other_tags.actor_tags.Contains(GameConstants.ActorTag.actor_can_press_switch))
        {
            is_pressed = true;
            switch_audio_source.pitch = 1;
            switch_audio_source.timeSamples = 0;
            switch_audio_source.Play();
            foreach (var switch_receiver in switch_receivers)
            {
                switch_receiver.OnActivate();
            }

            // if a game variable is set, update it.

            if (switch_game_var != null && switch_game_var != string.Empty)
                master_controller.data_controller.UpdateGameVar(switch_game_var, is_pressed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // no action if switch is permanently pressed.
        if (is_permanent)
            return;

        var other_tags = other.gameObject.GetComponent<ActorTagController>();
        if (other_tags == null)
            return;

        if (other_tags.actor_tags.Contains(GameConstants.ActorTag.actor_can_press_switch))
        {
            is_pressed = false;
            switch_audio_source.pitch = -1;
            switch_audio_source.timeSamples = switch_sound.samples - 1;
            switch_audio_source.Play();
            foreach (var switch_receiver in switch_receivers)
            {
                switch_receiver.OnDeactivate();
            }
        }
    }
}
