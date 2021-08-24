using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.script;

namespace Assets.script
{
    [System.Serializable]
    public class GameCutsceneEventData
    {
        public GameObject next_event_source = null;
        public GameEnums.GameCustsceneEventType event_type = GameEnums.GameCustsceneEventType.event_message_box;

        public string message_box_audio_vox = "default";
        public float message_box_audio_pitch = 1.0f;
        public string message_box_text = string.Empty;

        public GameObject set_fixed_camera_object;
    }
}
