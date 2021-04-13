using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class ActorFootstepController : MonoBehaviour
{
    const float AUDIO_MIN_PITCH = 0.9f;
    const float AUDIO_MAX_PITCH = 1.1f;

    public IActorFootstepManager manager;

    private (bool, string, float, bool, bool) input_tuple;
    private bool is_step_grounded = false;
    private string step_type = GameConstants.COLLIDER_TYPE_DEFAULT;
    private float step_speed = 0.0f;

    // step variables.

    public float step_interval = 35.0f;
    private float step_timer = 0.0f;
    private int step_number = 0;
    private int step_key = 0;
    private System.Random step_random = new System.Random();

    // water variables.

    private bool is_in_water = false;
    private bool is_submerged = false;

    // audio clip variables.

    AudioClip audio_step_default_1;
    AudioClip audio_step_default_2;

    AudioClip audio_step_water_1;
    AudioClip audio_step_water_2;

    AudioClip audio_step_sand_1;
    AudioClip audio_step_sand_2;

    AudioClip audio_step_stone_1;
    AudioClip audio_step_stone_2;

    AudioClip audio_step_grass_1;
    AudioClip audio_step_grass_2;

    AudioSource audio_source;

    // collection variables.

    Dictionary<(string,int), AudioClip> audio_library;

    private void Start()
    {
        manager = this.gameObject.GetComponent<IActorFootstepManager>();

        audio_step_default_1 = Resources.Load("sound/sfx_step_default_1") as AudioClip;
        audio_step_default_2 = Resources.Load("sound/sfx_step_default_2") as AudioClip;

        audio_step_water_1 = Resources.Load("sound/sfx_step_water_1") as AudioClip;
        audio_step_water_2 = Resources.Load("sound/sfx_step_water_2") as AudioClip;

        audio_step_sand_1 = Resources.Load("sound/sfx_step_sand_1") as AudioClip;
        audio_step_sand_2 = Resources.Load("sound/sfx_step_sand_2") as AudioClip;

        audio_step_stone_1 = Resources.Load("sound/sfx_step_stone_1") as AudioClip;
        audio_step_stone_2 = Resources.Load("sound/sfx_step_stone_2") as AudioClip;

        audio_step_grass_1 = Resources.Load("sound/sfx_step_grass_1") as AudioClip;
        audio_step_grass_2 = Resources.Load("sound/sfx_step_grass_2") as AudioClip;

        audio_source = gameObject.AddComponent<AudioSource>();
        audio_source.clip = audio_step_sand_1;
        audio_source.loop = false;
        audio_source.playOnAwake = false;
        audio_source.volume = 1.0f;

        audio_library = new Dictionary<(string,int), AudioClip>();

        audio_library.Add((GameConstants.COLLIDER_TYPE_DEFAULT,1), audio_step_default_1);
        audio_library.Add((GameConstants.COLLIDER_TYPE_DEFAULT,2), audio_step_default_2);

        audio_library.Add((GameConstants.COLLIDER_TYPE_WATER, 1), audio_step_water_1);
        audio_library.Add((GameConstants.COLLIDER_TYPE_WATER, 2), audio_step_water_2);


        audio_library.Add((GameConstants.COLLIDER_TYPE_GRASS, 1), audio_step_grass_1);
        audio_library.Add((GameConstants.COLLIDER_TYPE_GRASS, 2), audio_step_grass_2);

        audio_library.Add((GameConstants.COLLIDER_TYPE_SAND, 1), audio_step_sand_1);
        audio_library.Add((GameConstants.COLLIDER_TYPE_SAND, 2), audio_step_sand_2);

        audio_library.Add((GameConstants.COLLIDER_TYPE_STONE, 1), audio_step_stone_1);
        audio_library.Add((GameConstants.COLLIDER_TYPE_STONE, 2), audio_step_stone_2);
    }

    private void FixedUpdate()
    {
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        // Get the status from the manager.

        input_tuple = manager.UpdateFootstepController();

        is_step_grounded = input_tuple.Item1;
        step_type = input_tuple.Item2;
        step_speed = input_tuple.Item3;
        is_in_water = input_tuple.Item4;
        is_submerged = input_tuple.Item5;

        // Only do something if grounded.

        if (!is_step_grounded)
            return;

        // Do the footsteps.

        step_timer += step_speed;

        if (step_timer < step_interval)
            return;

        step_timer = 0.0f;

        // Play sounds conditionally.

        step_number++;
        step_key = (step_number % 2 == 0) ? 1 : 2;

        // Randomize the pitch slightly.

        audio_source.pitch = Random.Range(AUDIO_MIN_PITCH, AUDIO_MAX_PITCH);

        // Check for underwater, reduce pitch.

        audio_source.pitch = (is_submerged) 
            ? audio_source.pitch * 0.5f 
            : audio_source.pitch * 1.0f;

        // Check for water, set the type to water.

        if (is_in_water)
            step_type = GameConstants.COLLIDER_TYPE_WATER;

        // Play the sound.

        audio_source.clip = audio_library[(step_type, step_key)];
        audio_source.Play();
    }
}
