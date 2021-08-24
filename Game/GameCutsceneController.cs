using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using TMPro;

public class GameCutsceneController : MonoBehaviour
{
    public GameMasterController master;
    public GameUserInterfaceController ui;

    GameObject event_source;
    public GameCutsceneEventData current_event;

    float current_event_time = 0f;
    float current_event_time_interval = 1f;
    float current_event_step_time = 0f;
    float current_event_step_time_interval = 0.05f;

    bool is_current_event_item_started = false;
    bool is_current_event_item_finished = false;

    // text variables.

    int message_box_text_index = 0;
    public string message_box_text = string.Empty;
    char message_box_next_char = char.MinValue;
    AudioClip message_box_vox = null;

    // audio variables.

    AudioSource audio_source;

    // input variables.

    bool is_input_positive = false;
    bool was_input_positive = false;

    void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();
        ui = master.user_interface_controller;

        audio_source = this.gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (master.game_state != GameState.Cutscene)
            return;

        UpdateInput();

        // start event.
        if (!is_current_event_item_started)
            StartEventItem();

        current_event_time += Time.deltaTime;
        current_event_step_time += Time.deltaTime;

        if(current_event_step_time >= current_event_step_time_interval)
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

            if (current_event.next_event_source == null)
            {
                // return to game if there are no more items.
                EndCutscene();
            }
            else
            {
                // start the next cutscene event.
                StartCutscene(current_event.next_event_source);
            }
        }
    }

    private void UpdateInput()
    {
        was_input_positive = is_input_positive;
        is_input_positive = master.input_controller.action_positive.ReadValue<float>() >= 0.1f;
    }

    private void StartEventItem()
    {
        current_event_time = 0.0f;
        current_event_step_time = 0.0f;

        is_current_event_item_started = true;

        if (current_event.event_type == GameEnums.GameCustsceneEventType.event_message_box)
        {
            message_box_text = string.Empty;
            message_box_text_index = 0;
        }
        else if(current_event.event_type ==  GameEnums.GameCustsceneEventType.event_set_fixed_camera)
        {
            var change_data = new CameraModeChangeData();
            change_data.fixed_transform = current_event.set_fixed_camera_object.transform;
            var player_camera_object = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);
            player_camera_object.GetComponent<CameraController>().SetCamera(GameConstants.CameraMode.camera_fixed, change_data);
        }
        else if (current_event.event_type == GameEnums.GameCustsceneEventType.event_unset_fixed_camera)
        {
            var player_camera_object = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);
            player_camera_object.GetComponent<CameraController>().UnsetCamera();
        }
    }

    private void ProcessEventItem()
    {
        // handle the current event item.

        if(current_event.event_type == GameEnums.GameCustsceneEventType.event_message_box)
        {
            if(message_box_text_index < current_event.message_box_text.Length)
            {
                message_box_next_char = current_event.message_box_text[message_box_text_index];

                if (message_box_next_char == '<')
                {
                    // handle tag.

                    message_box_text += message_box_next_char;

                    while (message_box_next_char != '>')
                    {
                        message_box_text_index++;
                        message_box_next_char = current_event.message_box_text[message_box_text_index];
                        message_box_text += message_box_next_char;
                    }
                }
                else
                {
                    // handle regular char.

                    message_box_text += message_box_next_char;
                }

                if (message_box_text_index % 3 == 0)
                {
                    // play vox for every other letter.
                    PlayVox(current_event.message_box_audio_vox, message_box_text, message_box_text_index);
                }

                // increment to next character.
                message_box_text_index++;
            }
        }
    }

    private void FinishEventItem()
    {
        if (current_event == null)
            return;

        if (current_event.event_type == GameEnums.GameCustsceneEventType.event_message_box)
        {
            // finish event item if button is pressed.

            if (!was_input_positive && is_input_positive)
                is_current_event_item_finished = true;
        }
        else if (current_event.event_type == GameEnums.GameCustsceneEventType.event_set_fixed_camera)
        {
            if (current_event_time >= 1.0f)
                is_current_event_item_finished = true;
        }
        else if (current_event.event_type == GameEnums.GameCustsceneEventType.event_unset_fixed_camera)
        {
            if (current_event_time >= 1.0f)
            is_current_event_item_finished = true;
        }
    }

    public void StartCutscene(GameObject event_source)
    {
        master.ChangeState(GameState.Cutscene);

        this.event_source = event_source;
        this.current_event = event_source.GetComponent<EventController>().event_data;

        current_event_step_time = 0f;
    }

    public void EndCutscene()
    {
        master.ChangeState(GameState.Game);
    }

    public AudioClip PlayVox(string vox, string vox_text, int vox_index)
    {
        audio_source.clip = (vox_index % 6 == 0) 
            ? master.audio_controller.vox_dictionary[(vox,1)]
            : master.audio_controller.vox_dictionary[(vox,2)];

        audio_source.pitch = current_event.message_box_audio_pitch * Random.Range(1.0f, 1.25f);
        audio_source.Play();

        return null;
    }
}
