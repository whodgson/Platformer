using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class ActorFootstepController : MonoBehaviour
{
    const float AUDIO_MIN_PITCH = 0.9f;
    const float AUDIO_MAX_PITCH = 1.1f;

    public IActorFootstepManager manager;

    private ActorFootstepManager manager_data;
    private bool is_step_grounded = false;
    private GameConstants.GroundType step_type = GameConstants.GroundType.ground_default;
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

    AudioClip audio_step_foliage;

    AudioClip audio_step_wood_1;
    AudioClip audio_step_wood_2;

    AudioClip audio_step_mud;
    AudioClip audio_step_metal;

    AudioSource audio_source;

    // collection variables.

    Dictionary<(GameConstants.GroundType,int), AudioClip> audio_library;

    private void Start()
    {
        manager = this.gameObject.GetComponent<IActorFootstepManager>();

        audio_step_default_1 = Resources.Load("sound/actor_step/sfx_step_default_1") as AudioClip;
        audio_step_default_2 = Resources.Load("sound/actor_step/sfx_step_default_2") as AudioClip;

        audio_step_water_1 = Resources.Load("sound/actor_step/sfx_step_water_1") as AudioClip;
        audio_step_water_2 = Resources.Load("sound/actor_step/sfx_step_water_2") as AudioClip;

        audio_step_sand_1 = Resources.Load("sound/actor_step/sfx_step_sand_1") as AudioClip;
        audio_step_sand_2 = Resources.Load("sound/actor_step/sfx_step_sand_2") as AudioClip;

        audio_step_stone_1 = Resources.Load("sound/actor_step/sfx_step_stone_1") as AudioClip;
        audio_step_stone_2 = Resources.Load("sound/actor_step/sfx_step_stone_2") as AudioClip;

        audio_step_grass_1 = Resources.Load("sound/actor_step/sfx_step_grass_1") as AudioClip;
        audio_step_grass_2 = Resources.Load("sound/actor_step/sfx_step_grass_2") as AudioClip;

        audio_step_wood_1 = Resources.Load("sound/actor_step/sfx_step_wood_1") as AudioClip;
        audio_step_wood_2 = Resources.Load("sound/actor_step/sfx_step_wood_2") as AudioClip;

        audio_step_mud = Resources.Load("sound/actor_step/sfx_step_mud") as AudioClip;

        audio_step_metal = Resources.Load("sound/actor_step/sfx_step_metal") as AudioClip;

        audio_step_foliage = Resources.Load("sound/actor_step/sfx_step_foliage") as AudioClip;

        audio_source = gameObject.AddComponent<AudioSource>();
        audio_source.clip = audio_step_sand_1;
        audio_source.loop = false;
        audio_source.playOnAwake = false;
        audio_source.volume = 0.1f;

        audio_library = new Dictionary<(GameConstants.GroundType, int), AudioClip>();

        audio_library.Add((GameConstants.GroundType.ground_default,1), audio_step_default_1);
        audio_library.Add((GameConstants.GroundType.ground_default, 2), audio_step_default_2);

        audio_library.Add((GameConstants.GroundType.ground_slide, 1), audio_step_default_1);
        audio_library.Add((GameConstants.GroundType.ground_slide, 2), audio_step_default_2);

        audio_library.Add((GameConstants.GroundType.ground_water, 1), audio_step_water_1);
        audio_library.Add((GameConstants.GroundType.ground_water, 2), audio_step_water_2);

        audio_library.Add((GameConstants.GroundType.ground_grass, 1), audio_step_grass_1);
        audio_library.Add((GameConstants.GroundType.ground_grass, 2), audio_step_grass_2);

        audio_library.Add((GameConstants.GroundType.ground_sand, 1), audio_step_sand_1);
        audio_library.Add((GameConstants.GroundType.ground_sand, 2), audio_step_sand_2);

        audio_library.Add((GameConstants.GroundType.ground_stone, 1), audio_step_stone_1);
        audio_library.Add((GameConstants.GroundType.ground_stone, 2), audio_step_stone_2);

        audio_library.Add((GameConstants.GroundType.ground_wood, 1), audio_step_wood_1);
        audio_library.Add((GameConstants.GroundType.ground_wood, 2), audio_step_wood_2);

        audio_library.Add((GameConstants.GroundType.ground_mud, 1), audio_step_mud);
        audio_library.Add((GameConstants.GroundType.ground_mud, 2), audio_step_mud);

        audio_library.Add((GameConstants.GroundType.ground_metal, 1), audio_step_metal);
        audio_library.Add((GameConstants.GroundType.ground_metal, 2), audio_step_metal);

        audio_library.Add((GameConstants.GroundType.ground_foliage, 1), audio_step_foliage);
        audio_library.Add((GameConstants.GroundType.ground_foliage, 2), audio_step_foliage);
    }

    private void FixedUpdate()
    {
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        // Get the status from the manager.

        manager_data = manager.UpdateFootstepController();

        if (manager_data == null)
            return;

        is_step_grounded = manager_data.is_grounded;
        step_type = manager_data.ground_type;
        step_speed = manager_data.velocity;
        is_in_water = manager_data.is_in_water;
        is_submerged = manager_data.is_submerged;

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
            step_type = GameConstants.GroundType.ground_water;

        // Play the sound.

        audio_source.clip = audio_library[(step_type, step_key)];
        audio_source.Play();
    }
}
