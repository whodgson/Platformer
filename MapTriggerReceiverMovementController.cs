using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using System;

public class MapTriggerReceiverMovementController : MonoBehaviour
{
    private bool isActive = false;
    private float active_timer = 0.0f;

    public GameObject receiver_object;
    private MapTriggerReceiverController receiver;
    public GameObject move_object;
    public GameObject move_start_position;
    public GameObject move_end_position;
    public float move_speed_multiplier = 1f;
    public AudioClip move_sound = null;

    private AudioSource audio_source;

    private GameMasterController master_controller;

    private void Start()
    {
        master_controller = GameMasterController.GetMasterController();

        receiver = receiver_object.GetComponent<MapTriggerReceiverController>();

        receiver.Activate += OnSwitchPress;
        receiver.Deactivate += OnSwitchRelease;

        if (move_sound != null)
        {
            audio_source = gameObject.AddComponent<AudioSource>();
            audio_source.clip = move_sound;
            audio_source.loop = false;
            audio_source.playOnAwake = false;
            audio_source.volume = 1f * master_controller.audio_controller.volume_object;
        }
    }

    public void OnSwitchPress(object sender, EventArgs e)
    {
        isActive = true;
        if (move_sound != null)
        {
            audio_source.pitch = 1;
            audio_source.timeSamples = 0;
            audio_source.Play();
        }
    }

    public void OnSwitchRelease(object sender, EventArgs e)
    {
        isActive = false;
        if (move_sound != null)
        {
            audio_source.pitch = -1;
            audio_source.timeSamples = move_sound.samples - 1;
            audio_source.Play();
        }
    }

    void Update()
    {
        if (isActive)
        {
            if (active_timer < 1)
                active_timer += Time.deltaTime * move_speed_multiplier;
        }
        else
        {
            if (active_timer > 0)
                active_timer -= Time.deltaTime * move_speed_multiplier;
        }

        // if the timer is not resting, LERP the object
        // position between its start and end points.

        if (active_timer > 0 && active_timer < 1)
        {
            move_object.transform.position = Vector3.Lerp
                (move_start_position.transform.position,
                move_end_position.transform.position,
                active_timer);
        }
    }
}
