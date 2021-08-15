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
    public AudioClip a_player_slide;
    public AudioClip a_player_slide_loop;
    public AudioClip a_player_dive;
    public AudioClip a_player_hurt_default;

    private void Awake()
    {
        a_player_jump = Resources.Load("sound/player/sfx_player_jump") as AudioClip;
        a_player_water_jump = Resources.Load("sound/player/sfx_player_water_jump") as AudioClip;
        a_player_splash = Resources.Load("sound/player/sfx_player_splash") as AudioClip;
        a_player_slide  = Resources.Load("sound/player/sfx_player_slide") as AudioClip;
        a_player_slide_loop = Resources.Load("sound/player/sfx_player_slide_loop") as AudioClip;
        a_player_dive = Resources.Load("sound/player/sfx_player_dive") as AudioClip;
        a_player_hurt_default = Resources.Load("sound/player/sfx_player_hurt_default") as AudioClip;

    }
}
