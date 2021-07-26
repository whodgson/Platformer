using Assets.script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAudioController : MonoBehaviour
{
    public GameObject manager_game_object;
    private ICameraAudioManager manager;
    private CameraAudioManager manager_data;

    AudioLowPassFilter audio_low_pass_filter;

    // Start is called before the first frame update
    void Start()
    {
        manager_game_object = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
        manager = manager_game_object.GetComponent<ICameraAudioManager>();

        audio_low_pass_filter = gameObject.AddComponent<AudioLowPassFilter>();
        audio_low_pass_filter.enabled = false;
        audio_low_pass_filter.cutoffFrequency = 500f;
    }

    private void FixedUpdate()
    {
        manager_data = manager.UpdateCameraAudioController();
        audio_low_pass_filter.enabled = manager_data.is_submerged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
