using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    public float volume_music = 1.0f;
    public float volume_footstep = 1.0f;
    public float volume_object = 1.0f;

    public AudioClip a_player_jump;
    public AudioClip a_player_water_jump;
    public AudioClip a_player_splash;

    private void Awake()
    {
        a_player_jump = Resources.Load("sound/player/sfx_player_jump") as AudioClip;
        a_player_water_jump = Resources.Load("sound/player/sfx_player_water_jump") as AudioClip;
        a_player_splash = Resources.Load("sound/player/sfx_player_splash") as AudioClip;

    }
}
