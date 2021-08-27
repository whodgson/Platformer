using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.script.Event
{
    class EventMessageBoxController : MonoBehaviour, IEventController
    {
        // core variables.

        private GameMasterController master;
        public GameObject next_event_source = null;

        public string message_audio_vox = "default";
        public float message_audio_pitch = 1.0f;
        public string message_text = string.Empty;

        string output_text = string.Empty;
        char output_next_char = char.MinValue;
        int output_text_index = 0;

        // audio variables.

        AudioSource audio_source;

        AudioClip vox_clip = null;
        AudioClip[] vox_clip_array = null;
        int vox_clip_array_index = 0;

        // rng.

        System.Random sys_random;

        void Start()
        {
            master = GameMasterController.GetMasterController();
            audio_source = this.gameObject.AddComponent<AudioSource>();

            sys_random = new System.Random();
        }

        public GameObject GetNextEventSource()
        {
            return next_event_source;
        }

        public string GetEventType()
        {
            return GameConstants.EVENT_TYPE_MESSAGE_BOX;
        }

        public void StartEvent()
        {
            output_text = string.Empty;
            output_text_index = 0;
        }

        public void ProcessEvent()
        {
            if (output_text_index < message_text.Length)
            {
                output_next_char = message_text[output_text_index];

                if (output_next_char == '<')
                {
                    // handle tag.

                    output_text += output_next_char;

                    while (output_next_char != '>')
                    {
                        output_text_index++;
                        output_next_char = message_text[output_text_index];
                        output_text += output_next_char;
                    }
                }
                else
                {
                    // handle regular char.

                    output_text += output_next_char;
                }

                if (output_text_index % 3 == 0)
                {
                    // play vox for every other letter.
                    PlayVox(message_audio_vox, message_text, output_text_index);
                }

                // increment to next character.
                output_text_index++;
            }

            master.cutscene_controller.message_box_text = output_text;
        }

        public bool FinishEvent()
        {
            // if the button is pressed, and
            // reached the end of the message.

            if (!master.input_controller.was_input_positive 
                && master.input_controller.is_input_positive
                && output_text_index == message_text.Length)
                return true;

            return false;
        }

        public AudioClip PlayVox(string vox, string vox_text, int vox_index)
        {
            // work through the audio clips for this vox.

            vox_clip_array = master.audio_controller.vox_dictionary[vox];

            if (vox_clip_array_index == vox_clip_array.Length)
                vox_clip_array_index = 0;

            //vox_clip = vox_clip_array[vox_clip_array_index];
            vox_clip = vox_clip_array[sys_random.Next(0, vox_clip_array.Length - 1)];
            vox_clip_array_index++;

            // play the clip.

            audio_source.clip = vox_clip;
            audio_source.pitch = message_audio_pitch * UnityEngine.Random.Range(1.0f, 1.25f);
            audio_source.Play();

            return null;
        }

        
    }
}
