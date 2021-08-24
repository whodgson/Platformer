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
    public AudioClip a_player_hurt_fire;

    // vox.

    public AudioClip vox_default_1;
    public AudioClip vox_default_2;

    public AudioClip vox_depressed_1;
    public AudioClip vox_depressed_2;

    public Dictionary<(string,int), AudioClip> vox_dictionary;

    private void Awake()
    {
        a_player_jump = Resources.Load("sound/player/sfx_player_jump") as AudioClip;
        a_player_water_jump = Resources.Load("sound/player/sfx_player_water_jump") as AudioClip;
        a_player_splash = Resources.Load("sound/player/sfx_player_splash") as AudioClip;
        a_player_slide  = Resources.Load("sound/player/sfx_player_slide") as AudioClip;
        a_player_slide_loop = Resources.Load("sound/player/sfx_player_slide_loop") as AudioClip;
        a_player_dive = Resources.Load("sound/player/sfx_player_dive") as AudioClip;
        a_player_hurt_default = Resources.Load("sound/player/sfx_player_hurt_default") as AudioClip;
        a_player_hurt_fire = Resources.Load("sound/player/sfx_player_hurt_fire") as AudioClip;

        // load vox.

        vox_default_1 = Resources.Load("sound/vox/vox_default_1") as AudioClip;
        vox_default_2 = Resources.Load("sound/vox/vox_default_2") as AudioClip;

        vox_depressed_1 = Resources.Load("sound/vox/vox_depressed_1") as AudioClip;
        vox_depressed_2 = Resources.Load("sound/vox/vox_depressed_2") as AudioClip;
        
        vox_dictionary = new Dictionary<(string,int), AudioClip>();

        vox_dictionary.Add(("default",1), vox_default_1);
        vox_dictionary.Add(("default",2), vox_default_2);

        vox_dictionary.Add(("depressed", 1), vox_depressed_1);
        vox_dictionary.Add(("depressed", 2), vox_depressed_2);
    }
}
