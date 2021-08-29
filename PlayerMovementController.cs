using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using System;

public class PlayerMovementController : MonoBehaviour
    , IActorFootstepManager
    , ICameraAudioManager
    , IActorSplashManager
    , IActorDamageEffectManager
{
    // state constants.

    public enum PlayerState
    {
        player_default,
        player_jump,
        player_water_default,
        player_water_jump,
        player_slide,
        player_dive,
        player_attack,
        player_damage,
    }

    // input constants.

    const float INPUT_GAME_STATE_DELAY = 0.5f;
    const float INPUT_DIRECTIONAL_THRESHOLD = 0.01f;
    const float INPUT_BUTTON_THRESHOLD = 0.5f;

    // component constants.

    const string MAIN_COLLIDER_GAME_OBJECT_NAME = "main_collider";
    const string PLAYER_RENDER_GAME_OBJECT_NAME = "player_render";

    // physical constants.

    const float RIGID_BODY_MASS = 1f;
    const float RIGID_BODY_DRAG = 1f;
    const float RIGID_BODY_ANGULAR_DRAG = 0.05f;

    const float GRAVITY_MULTIPLIER = 2.0f;

    const float DRAG_GROUNDED = 5f;
    const float DRAG_AIR = 1f;

    const float ACCELERATION_GROUNDED = 0.5f;
    const float ACCELERATION_AIR = 0.2f;
    
    const float MAX_SPEED_GROUNDED = 3.0f;
    const float MAX_SPEED_WATER = 2.0f;

    const float MAX_SPEED_SLIDE = 6.0f;
    const float MAX_SPEED_DIVE = 7.0f;

    // grounded constants.

    const float GROUNDED_RAYCAST_ADDITIONAL_DISTANCE = 0.1f;
    const float GROUNDED_SPHERECAST_ADDITIONAL_DISTANCE = 0.025f;

    const float GROUNDED_RAYCAST_DISTANCE = 100f;

    const float GROUNDED_SPHERECAST_RADIUS = 0.187f;

    // movement constants.

    readonly Vector3 STEP_MOVEMENT_OFFSET = new Vector3(0, 0.15f, 0);
    const float STEP_MAX_VELOCITY = 1f;

    const float MOVEMENT_SPHERECAST_DISTANCE = 0.1f;

    // jump constants.

    const float JUMP_FORCE_MULTIPLIER = 3.0f;
    const float JUMP_PERSIST_FORCE_MULTIPLIER = 0.4f;
    const int JUMP_PERSIST_ENERGY_MAX = 10;

    const float WATER_JUMP_FORCE_MULTIPLIER = 2.5f;
    const float MINIMUM_WATER_JUMP_Y_SPEED = -1f;

    const int UPDATE_COUNT_JUMP_RECOVERY_MIN = 10;

    // slide constants.

    const float SLIDE_FORCE_ANGLE_MIN = 3f;                         // minimum angle to adjust slide direction to slope.
    const float SLIDE_ANGLE_RECOVERY_MAX = 30f;                     // maximum angle to recover from slide.
    const float SLIDE_SPEED_RECOVERY_MAX = 0.25f;                   // maximum speed to recover from slide.
    const float SLIDE_ANGLE_MIN = 50f;                              // minimum angle to start sliding
    const float SLIDE_RESISTANCE_GROUND_ANGLE_MULTIPLIER = 0.001f;  // multiplier for ground angle to subtract from resistance.
    const float SLIDE_RESISTANCE_MAX = 1.0f;                        // maximum slide resistance.
    const float SLIDE_RESISTANCE_RECOVERY = 0.05f;                  // slide resistance recovery amount
    const float SLIDE_FORCE_MULIPLIER = 1f;                         // multiplier to slide vector.
    const float SLIDE_DIRECTION_ROTATION_MULTIPLIER = 0.5f;         // how fast the slide direction matches current slope.

    // dive constants.

    const float DIVE_MIN_INPUT_DIRECTIONAL_MAGNITUDE = 1.0f;        // minimum input magnitude to influence dive direction.
    const int UPDATE_COUNT_DIVE_RECOVERY_MIN = 30;                  // minimum update count to recover from dive.

    // water constants.

    readonly Vector3 WATER_PARTIAL_SUBMERGED_OFFSET = new Vector3(0, 0, 0);
    readonly Vector3 WATER_FULL_SUBMERGED_OFFSET = new Vector3(0, 0.1625f, 0);

    // animation constants.

    const float ANIMATION_TURNING_SPEED_MULTIPLIER = 0.5f;

    // state variables.

    PlayerState player_state = PlayerState.player_default;
    PlayerState player_state_previous = PlayerState.player_default;

    // update count variables.

    int update_count_default = 0;
    int update_count_jump = 0;
    int update_count_water_default = 0;
    int update_count_water_jump = 0;
    int update_count_slide = 0;
    int update_count_dive = 0;
    int update_count_attack = 0;
    int update_count_damage = 0;

    // input variables.

    Vector3 input_directional = Vector3.zero;
    bool is_input_directional = false;
    bool was_input_directional = false;

    bool is_input_left = false;
    bool is_input_right = false;

    bool is_input_jump = false;
    bool was_input_jump = false;

    bool is_input_attack = false;
    bool was_input_attack = false;

    // component variables.

    private GameMasterController master;
    private Rigidbody rigid_body;
    private SphereCollider player_sphere_collider;
    private Animator player_animator;
    private GameObject camera_object;
    private GameObject player_render;
    private Renderer player_renderer;

    // grounded variables.

    float grounded_raycast_max_distance = 0f;
    float grounded_spherecast_max_distance = 0f;

    RaycastHit raycast_hit_info;
    bool is_raycast_hit = false;
    bool is_raycast_grounded = false;

    RaycastHit spherecast_hit_info;
    bool is_spherecast_hit = false;
    bool is_spherecast_grounded = false;

    float raycast_grounded_slope_angle = 0.0f;
    Vector3 raycast_grounded_slope_normal = Vector3.up;
    Vector3 raycast_grounded_slope_direction = Vector3.up;

    float spherecast_grounded_slope_angle = 0.0f;
    Vector3 spherecast_grounded_slope_normal = Vector3.up;

    GameConstants.GroundType ground_type;

    // movement variables.

    RaycastHit movement_hit;
    RaycastHit step_movement_hit;

    bool is_movement_hit = false;
    bool is_step_movement_hit = false;

    // jump variables.

    int jump_persist_energy = 0;

    // slide variables.

    float slide_resistance = 1.0f;
    float slide_force = 1.0f;
    Vector3 slide_direction = Vector3.zero;

    bool is_slide_hit = false;

    // dive variables.

    Vector3 dive_direction = Vector3.zero;

    // damage variables.

    GameObject damage_source = null;
    AttributeDamageTypeData damage_type = null;

    // moving object variables.

    List<GameObject> moving_object_collision_list = new List<GameObject>();
    bool is_colliding_moving_object = false;

    // water variables.

    List<GameObject> water_object_collision_list = new List<GameObject>();
    bool is_colliding_water_object = false;
    bool is_partial_submerged = false;
    bool is_full_submerged = false;
    float water_y_level = 0;

    // animator variables.

    Vector3 facing_direction = Vector3.zero;
    Vector3 facing_direction_delta = Vector3.zero;

    public RuntimeAnimatorController animator_game;
    public RuntimeAnimatorController animator_game_over;
    public RuntimeAnimatorController animator_cutscene;

    // audio variables.

    AudioSource audio_source;
    AudioSource audio_source_loop;

    // interface variables.

    CameraAudioManager camera_audio_manager;
    ActorFootstepManager footstep_manager;
    ActorSplashManager splash_manager;
    ActorDamageEffectManager damage_effect_manager;
      
    private void Start()
    {
        master = GameMasterController.GetMasterController();

        // initialise componenets.

        rigid_body = this.GetComponent<Rigidbody>();
        player_sphere_collider = GameObject.Find
            (MAIN_COLLIDER_GAME_OBJECT_NAME).GetComponent<SphereCollider>();
        player_animator = this.GetComponent<Animator>();
        camera_object = GameObject.FindGameObjectWithTag(GameConstants.TAG_MAIN_CAMERA);
        player_render = GameObject.Find(PLAYER_RENDER_GAME_OBJECT_NAME);
        player_renderer = this.GetComponentInChildren<SkinnedMeshRenderer>();

        // add listeners.

        master.GameStateChange += ChangeGameState;

        // add components.

        audio_source = this.gameObject.AddComponent<AudioSource>();

        audio_source_loop = this.gameObject.AddComponent<AudioSource>();
        audio_source_loop.loop = true;

        // initialise actor attributes.

        damage_type = new AttributeDamageTypeData();

        // initialise interface.

        camera_audio_manager = new CameraAudioManager();
        footstep_manager = new ActorFootstepManager();
        splash_manager = new ActorSplashManager();
        damage_effect_manager = new ActorDamageEffectManager();

        // setup.

        InitialisePhysicalParameters();
    }

    void OnDestroy()
    {
        // remove listeners.

        master.GameStateChange -= ChangeGameState;
    }

    private void InitialisePhysicalParameters()
    {
        // rigid body inits.

        rigid_body.mass = RIGID_BODY_MASS;
        rigid_body.drag = RIGID_BODY_DRAG;
        rigid_body.angularDrag = RIGID_BODY_ANGULAR_DRAG;

        rigid_body.constraints = RigidbodyConstraints.FreezeRotation;

        // collider inits.

        player_sphere_collider.material.bounceCombine = PhysicMaterialCombine.Minimum;
        player_sphere_collider.material.bounciness = 0.0f;

        grounded_raycast_max_distance = player_sphere_collider.radius + GROUNDED_RAYCAST_ADDITIONAL_DISTANCE;
        grounded_spherecast_max_distance = GROUNDED_SPHERECAST_ADDITIONAL_DISTANCE;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdatePlayerInput();

        if (master.game_state == GameState.Game)
        {
            // run update if in game state.

            rigid_body.isKinematic = false;
            player_animator.enabled = true;

            UpdateWaterStatus();
            UpdateMovingObjectStatus();

            UpdateGroundedRay();
            UpdateGroundedSphere();
            UpdateGravity();
            UpdateDragAndFriction();

            // update the player state.

            if (player_state == PlayerState.player_default)
                UpdateDefaultPlayerState();
            else if (player_state == PlayerState.player_jump)
                UpdateJumpPlayerState();
            else if (player_state == PlayerState.player_water_default)
                UpdateWaterDefaultPlayerState();
            else if (player_state == PlayerState.player_water_jump)
                UpdateWaterJumpPlayerState();
            else if (player_state == PlayerState.player_slide)
                UpdateSlidePlayerState();
            else if (player_state == PlayerState.player_dive)
                UpdateDivePlayerState();
            else if (player_state == PlayerState.player_attack)
                UpdateAttackPlayerState();
            else if (player_state == PlayerState.player_damage)
                UpdateDamagePlayerState();

            // do state specific actions.

            if (player_state == PlayerState.player_default)
            {
                UpdateDefaultMovement();
                UpdateDefaultSlide();
                UpdateDefaultSpeed();
            }
            else if(player_state == PlayerState.player_jump)
            {
                UpdateJumpJump();
                UpdateJumpMovement();
                UpdateJumpSpeed();
            }
            else if(player_state == PlayerState.player_water_default)
            {
                UpdateDefaultMovement();
                UpdateWaterDefaultSpeed();
            }
            else if(player_state == PlayerState.player_water_jump)
            {
                UpdateJumpJump();
                UpdateJumpMovement();
                UpdateWaterJumpSpeed();
            }
            else if(player_state == PlayerState.player_slide)
            {
                UpdateSlideMovement();
                UpdateSlideLateralMovement();
                UpdateSlideSlide();
                UpdateSlideSpeed();
            }
            else if(player_state == PlayerState.player_dive)
            {
                UpdateDiveSpeed();
            }
            else if(player_state == PlayerState.player_attack)
            {

            }
            else if(player_state == PlayerState.player_damage)
            {
                UpdateJumpMovement();
            }

            // update animator.

            UpdateAnimator();
        }
        else if(master.game_state == GameState.GameOver)
        {
            rigid_body.isKinematic = true;
            UpdateAnimator();
        }
        else if(master.game_state == GameState.Cutscene)
        {
            rigid_body.isKinematic = true;
            UpdateAnimatorCutscene();
        }
        else
        {
            // freeze player.

            rigid_body.isKinematic = true;
            player_animator.enabled = false;
        }
    }

    private void UpdatePlayerInput()
    {
        // ignore all input for a short
        // time when entering game state.

        if (master.Game_State_Time <= INPUT_GAME_STATE_DELAY)
            return;

        // get previous inputs.

        was_input_directional = is_input_directional;
        was_input_jump = is_input_jump;
        was_input_attack = is_input_attack;

        // get input from input mapper.

        float input_horizontal = master.input_controller.action_horizontal.ReadValue<float>();
        float input_vertical = master.input_controller.action_vertical.ReadValue<float>();

        Vector3 input = new Vector3(input_horizontal, 0.0f, input_vertical);

        if (input.magnitude > 1)
            input = input.normalized;

        input_directional = input;
        is_input_directional = input_directional.magnitude > INPUT_DIRECTIONAL_THRESHOLD;

        is_input_left = master.input_controller.action_horizontal.ReadValue<float>() < -0.5f;
        is_input_right = master.input_controller.action_horizontal.ReadValue<float>() > 0.5f;

        // get button inputs.

        is_input_jump = master.input_controller.action_positive.ReadValue<float>() >= INPUT_BUTTON_THRESHOLD;
        is_input_attack = master.input_controller.action_interact.ReadValue<float>() >= INPUT_BUTTON_THRESHOLD;
    }

    private void UpdateWaterStatus()
    {
        if (!is_colliding_water_object)
            return;

        is_partial_submerged = (this.transform.position + WATER_PARTIAL_SUBMERGED_OFFSET).y <= water_y_level;
        is_full_submerged = (this.transform.position + WATER_FULL_SUBMERGED_OFFSET).y <= water_y_level;
    }

    private void UpdateMovingObjectStatus()
    {
        if (!is_colliding_moving_object)
            return;
    }

    private void UpdateGroundedRay()
    {
        // check grounding by ray.

        is_raycast_hit = Physics.Raycast(this.transform.position, Vector3.down, out raycast_hit_info, GROUNDED_RAYCAST_DISTANCE);

        if(is_raycast_hit)
        {
            // set grounded, if under max distance.
            is_raycast_grounded = raycast_hit_info.distance <= grounded_raycast_max_distance;

            // set the angle of the surface.
            raycast_grounded_slope_angle = Vector3.Angle(raycast_hit_info.normal, Vector3.up);

            // set the slope normal.
            raycast_grounded_slope_normal = raycast_hit_info.normal;

            // set the ground slope direction.
            var temp = Vector3.Cross(raycast_hit_info.normal, Vector3.down);
            raycast_grounded_slope_direction = Vector3.Cross(temp, raycast_hit_info.normal);

            // if grounded, and in the regular state, set the position above the floor.

            if (is_raycast_grounded && player_state == PlayerState.player_default)
            {
                rigid_body.MovePosition(new Vector3(
                    rigid_body.position.x,
                    raycast_hit_info.point.y + raycast_hit_info.distance,
                    rigid_body.position.z));
                Debug.DrawRay(transform.position, Vector3.down, Color.magenta);
            }
        }
    }

    private void UpdateGroundedSphere()
    {
        // check grounding by sphere.

        is_spherecast_hit = Physics.SphereCast(transform.position, GROUNDED_SPHERECAST_RADIUS, Vector3.down, out spherecast_hit_info, GROUNDED_RAYCAST_DISTANCE);

        if(is_spherecast_hit)
        {
            is_spherecast_grounded = spherecast_hit_info.distance <= grounded_spherecast_max_distance;

            // if also ray grounded
            // set the slope normal.
            spherecast_grounded_slope_normal = (is_raycast_grounded) 
                ? spherecast_hit_info.normal 
                : raycast_hit_info.normal;

            // if also ray grounded
            // set the angle of the surface.
            spherecast_grounded_slope_angle = (is_raycast_grounded) 
                ? Vector3.Angle(raycast_hit_info.normal, Vector3.up) 
                : raycast_grounded_slope_angle;

            // set the grounded type, if grounded.
            if (is_spherecast_grounded)
            {
                if (spherecast_hit_info.collider.gameObject
                    .GetComponent<MapAttributeGroundType>() == null)
                    ground_type = GameConstants.GroundType.ground_default;
                else
                    ground_type = spherecast_hit_info.collider.gameObject
                        .GetComponent<MapAttributeGroundType>().ground_type;
            }
            else
            {
                ground_type = GameConstants.GroundType.ground_default;
            }
        }
    }

    private void UpdateGravity()
    {
        // apply rising or falling gravity.

        if (rigid_body.velocity.y > 0)
            rigid_body.AddForce(Physics.gravity, ForceMode.Acceleration);
        else
            rigid_body.AddForce(Physics.gravity * GRAVITY_MULTIPLIER, ForceMode.Acceleration);
    }

    private void UpdateDragAndFriction()
    {
        // state specific drag.

        if(player_state == PlayerState.player_jump
            ||player_state == PlayerState.player_slide
            || player_state == PlayerState.player_dive
            || player_state == PlayerState.player_damage)
        {
            rigid_body.drag = DRAG_AIR;
            player_sphere_collider.material.dynamicFriction = 0f;
            player_sphere_collider.material.staticFriction = 0f;
            player_sphere_collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
            return;
        }

        // update based on circumstances.

        rigid_body.drag = is_raycast_grounded ? DRAG_GROUNDED : DRAG_AIR;

        if (is_input_directional || !is_raycast_grounded)
        {
            player_sphere_collider.material.dynamicFriction = 0f;
            player_sphere_collider.material.staticFriction = 0f;
        }
        else
        {
            if (moving_object_collision_list.Count == 0)
            {
                player_sphere_collider.material.dynamicFriction = 100;
                player_sphere_collider.material.staticFriction = 100;
            }
            else
            {
                player_sphere_collider.material.dynamicFriction = 1;
                player_sphere_collider.material.staticFriction = 1;
            }
        }

        // change physics combine mode.

        player_sphere_collider.material.frictionCombine = PhysicMaterialCombine.Average;
    }

    #region default

    private void UpdateDefaultPlayerState()
    {
        update_count_default++;

        // enter jumping state if right criteria are met.

        if(!was_input_jump && is_input_jump && is_spherecast_grounded)
        {
            ChangePlayerState(PlayerState.player_jump);
            return;
        }

        if (is_partial_submerged)
        {
            ChangePlayerState(PlayerState.player_water_default);
            return;
        }

        if (slide_resistance <= 0.0f)
        {
            ChangePlayerState(PlayerState.player_slide);
            return;
        }

        // exit to diving state, if the previous state
        // was jump, attack is pressed, and in air.

        if (!was_input_attack 
            && is_input_attack
            && player_state_previous == PlayerState.player_jump
            && !is_spherecast_grounded)
        {
            ChangePlayerState(PlayerState.player_dive);
            return;
        }
    }

    private void UpdateDefaultBegin()
    {
        update_count_default = 0;
    }

    private void UpdateDefaultMovement()
    {
        // input movement relative to camera.

        var camera_relative_movement = Quaternion.Euler(0, camera_object.transform.eulerAngles.y, 0) * input_directional;

        // camera movement relative to slope.

        var slope_relative_movement = Vector3.ProjectOnPlane(camera_relative_movement, spherecast_grounded_slope_normal);

        // acceleration

        float acceleration = is_spherecast_grounded ? ACCELERATION_GROUNDED : ACCELERATION_AIR;

        // force

        var force = slope_relative_movement * acceleration;

        // do raycasts.

        is_movement_hit = Physics.SphereCast
            (this.transform.position, GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

        Debug.DrawRay(transform.position, slope_relative_movement, Color.red);

        // apply forces based on raycast hits.

        if (is_movement_hit)
        {
            // initial step cast.
            is_step_movement_hit = Physics.SphereCast
                (this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out step_movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

            // addition step check for ceilings.
            if (Physics.CheckSphere(this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, GameConstants.LAYER_MASK_ALL_BUT_PLAYER))
                is_step_movement_hit = true;

            if (!is_step_movement_hit)
            {
                // step obstace, move up and move directly ahead.

                if (rigid_body.velocity.y < STEP_MAX_VELOCITY)
                    rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);

                rigid_body.AddForce(force, ForceMode.VelocityChange);

                // force the sphere grounded status while moving up short steps.
                is_spherecast_grounded = true;

                Debug.DrawRay(this.transform.position, Vector3.up, Color.magenta);
            }
            else
            {
                // full obstacle, move on a plane to the collided surface.

                force = Vector3.ProjectOnPlane(force, movement_hit.normal);
                rigid_body.AddForce(force, ForceMode.VelocityChange);

                Debug.DrawRay(transform.position, force, Color.blue);
            }
        }
        else
        {
            // no obstace directly ahead.

            rigid_body.AddForce(force, ForceMode.VelocityChange);

            Debug.DrawRay(this.transform.position, Vector3.up, Color.green);
        }

    }

    private void UpdateDefaultSlide()
    {
        // move towards the sliding state
        // if the right criteria are met.

        if(raycast_grounded_slope_angle > SLIDE_ANGLE_MIN 
            || ground_type == GameConstants.GroundType.ground_slide)
        {
            // reduce the slide resistance
            slide_resistance -= (raycast_grounded_slope_angle * SLIDE_RESISTANCE_GROUND_ANGLE_MULTIPLIER);
        }
        else
        {
            slide_resistance = SLIDE_RESISTANCE_MAX;
        }

        slide_resistance = Mathf.Clamp(slide_resistance, 0.0f, SLIDE_RESISTANCE_MAX);
    }

    private void UpdateDefaultSpeed()
    {
        // limit speed while in the default state.
        // maximum speed depends on criteria.

        if (is_partial_submerged)
        {
            if (rigid_body.velocity.magnitude > MAX_SPEED_WATER)
            {
                rigid_body.velocity = Vector3.ClampMagnitude(rigid_body.velocity, MAX_SPEED_WATER);
            }
        }
        else
        {
            if (is_spherecast_grounded)
            {
                if (rigid_body.velocity.magnitude > MAX_SPEED_GROUNDED)
                {
                    rigid_body.velocity = Vector3.ClampMagnitude(rigid_body.velocity, MAX_SPEED_GROUNDED);
                }
            }
            else
            {
                Vector3 old_x_z = new Vector3(rigid_body.velocity.x, 0, rigid_body.velocity.z);
                Vector3 old_y = new Vector3(0, rigid_body.velocity.y, 0);

                if (old_x_z.magnitude > MAX_SPEED_GROUNDED)
                {

                    old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_GROUNDED);
                    rigid_body.velocity = old_x_z + old_y;
                }
            }
        }
    }

    #endregion
    #region jump

    private void UpdateJumpPlayerState()
    {
        // increment the update count.

        update_count_jump++;

        // enter default state if right criteria are met.

        if (rigid_body.velocity.y <= 0)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }

        // enter default state if grounded.

        if(update_count_jump >= UPDATE_COUNT_JUMP_RECOVERY_MIN
            && (is_raycast_grounded || is_spherecast_grounded))
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }

        // enter dive state.

        if(!was_input_attack && is_input_attack)
        {
            ChangePlayerState(PlayerState.player_dive);
            return;
        }
    }

    private void UpdateJumpBegin()
    {
        // reset the update count.

        update_count_jump = 0;

        // if coming from the water jump state,
        // don't add any additional force.

        if (player_state_previous == PlayerState.player_water_jump)
            return;

        // enter jump state.
        // reset jump power.

        jump_persist_energy = JUMP_PERSIST_ENERGY_MAX;

        // add jumping force.

        rigid_body.velocity = new Vector3
            (rigid_body.velocity.x, 0, rigid_body.velocity.z);

        rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);

        // player sound.

        audio_source.clip = master.audio_controller.a_player_jump;
        audio_source.Play();
    }

    private void UpdateJumpJump()
    {
        // decrement the remaining jump persist energy.

        jump_persist_energy -= 1;

        if(is_input_jump && jump_persist_energy > 0)
        {
            // if the jump input is given, and persist energy > 0, add extra jump force.

            rigid_body.AddForce(Vector3.up * JUMP_PERSIST_FORCE_MULTIPLIER, ForceMode.VelocityChange);
        }
        else
        {
            // if the jump input is let go, zero out the jump persist energy.

            jump_persist_energy = 0;
        }
    }

    private void UpdateJumpMovement()
    {
        // input movement relative to camera.

        var camera_relative_movement = Quaternion.Euler(0, camera_object.transform.eulerAngles.y, 0) * input_directional;

        // force

        var force = camera_relative_movement * ACCELERATION_AIR;

        // do raycasts.

        is_movement_hit = Physics.SphereCast
            (this.transform.position, GROUNDED_SPHERECAST_RADIUS, camera_relative_movement, out movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

        // apply forces based on raycast hits.

        if (is_movement_hit)
        {
            // initial step cast.
            is_step_movement_hit = Physics.SphereCast
                (this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, camera_relative_movement, out step_movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

            // second step check for ceilings.
            if (Physics.CheckSphere(this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, GameConstants.LAYER_MASK_ALL_BUT_PLAYER))
                is_step_movement_hit = true;

            if (!is_step_movement_hit)
            {
                // step obstace, move up and move directly ahead.

                if (rigid_body.velocity.y < STEP_MAX_VELOCITY)
                    rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);

                rigid_body.AddForce(force, ForceMode.VelocityChange);
            }
            else
            {
                // full obstacle, move on a plane to the collided surface.

                force = Vector3.ProjectOnPlane(force, movement_hit.normal);
                rigid_body.AddForce(force, ForceMode.VelocityChange);

                Debug.DrawRay(transform.position, force, Color.blue);
            }
        }
        else
        {
            // no obstace directly ahead.

            rigid_body.AddForce(force, ForceMode.VelocityChange);
        }
    }

    private void UpdateJumpSpeed()
    {
        Vector3 old_x_z = new Vector3(rigid_body.velocity.x, 0, rigid_body.velocity.z);
        Vector3 old_y = new Vector3(0, rigid_body.velocity.y, 0);

        if (old_x_z.magnitude > MAX_SPEED_GROUNDED)
        {

            old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_GROUNDED);
            rigid_body.velocity = old_x_z + old_y;
        }
    }

    #endregion
    #region default water

    private void UpdateWaterDefaultPlayerState()
    {
        update_count_water_default++;

        // can swim if pressing jump 
        // and grounded, or descending in water.

        if 
        (
            !was_input_jump 
            && is_input_jump
            && (is_spherecast_grounded || rigid_body.velocity.y <= MINIMUM_WATER_JUMP_Y_SPEED)
        )
        {
            ChangePlayerState(PlayerState.player_water_jump);
            return;
        }

        if (!is_partial_submerged)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }
    }

    private void UpdateWaterDefaultBegin()
    {
        update_count_water_default = 0;
    }

    private void UpdateWaterDefaultSpeed()
    {
        if (rigid_body.velocity.magnitude > MAX_SPEED_WATER)
        {
            rigid_body.velocity = Vector3.ClampMagnitude(rigid_body.velocity, MAX_SPEED_WATER);
        }
    }

    #endregion
    #region jump water

    private void UpdateWaterJumpPlayerState()
    {
        update_count_water_jump++;

        if (rigid_body.velocity.y <= 0)
        {
            ChangePlayerState(PlayerState.player_water_default);
            return;
        }

        if (!is_partial_submerged)
        {
            ChangePlayerState(PlayerState.player_jump);
            return;
        }
    }

    private void UpdateWaterJumpBegin()
    {
        update_count_water_jump = 0;

        // enter jump state.
        // reset jump power.

        jump_persist_energy = JUMP_PERSIST_ENERGY_MAX;

        // add jumping force.

        rigid_body.velocity = new Vector3
            (rigid_body.velocity.x, 0, rigid_body.velocity.z);

        rigid_body.AddForce(Vector3.up * WATER_JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);

        // player sound.

        audio_source.clip = master.audio_controller.a_player_water_jump;
        audio_source.Play();
    }

    private void UpdateWaterJumpSpeed()
    {
        Vector3 old_x_z = new Vector3(rigid_body.velocity.x, 0, rigid_body.velocity.z);
        Vector3 old_y = new Vector3(0, rigid_body.velocity.y, 0);

        if (old_x_z.magnitude > MAX_SPEED_WATER)
        {
            old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_WATER);
        }

        if(old_y.y < -MAX_SPEED_WATER)
        {
            old_y = Vector3.ClampMagnitude(old_y, MAX_SPEED_WATER);
        }

        rigid_body.velocity = old_x_z + old_y;
    }

    #endregion
    #region slide

    private void UpdateSlidePlayerState()
    {
        update_count_slide++;

        // exit if entering water.
        if (is_partial_submerged)
        {
            ChangePlayerState(PlayerState.player_water_default);
            return;
        }

        // exit if slide resistance recovered.
        if (slide_resistance >= SLIDE_RESISTANCE_MAX)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }

        // exit if fully in air.
        if(!is_spherecast_grounded && !is_raycast_grounded)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }

        // exit if slow enough to jump.
        if (!was_input_jump 
            && is_input_jump 
            && is_spherecast_grounded
            && rigid_body.velocity.magnitude < SLIDE_SPEED_RECOVERY_MAX)
        {
            ChangePlayerState(PlayerState.player_jump);
            return;
        }
    }

    private void UpdateSlideBegin()
    {
        update_count_slide = 0;

        slide_force = SLIDE_FORCE_MULIPLIER;
        slide_direction = raycast_grounded_slope_direction;
        rigid_body.AddForce(raycast_grounded_slope_direction * slide_force, ForceMode.VelocityChange);

        audio_source.clip = master.audio_controller.a_player_slide;
        audio_source.Play();
    }

    private void UpdateSlideMovement()
    {
        // slide forward.

        if (raycast_grounded_slope_angle >= SLIDE_FORCE_ANGLE_MIN)
        {
            // update the slide vector.

            slide_direction = Vector3.RotateTowards
                (slide_direction, raycast_grounded_slope_direction.normalized, SLIDE_DIRECTION_ROTATION_MULTIPLIER, 0.0f);

            // add the regular slide force, if no obstacle.

            is_slide_hit = Physics.SphereCast
                (this.transform.position, GROUNDED_SPHERECAST_RADIUS, slide_direction, out movement_hit, MOVEMENT_SPHERECAST_DISTANCE);


            if (!is_slide_hit)
            {
                rigid_body.AddForce(slide_direction * SLIDE_FORCE_MULIPLIER, ForceMode.VelocityChange);
            }
        }
    }

    private void UpdateSlideLateralMovement()
    {
        // add sideways slide force.

        if (raycast_grounded_slope_angle >= SLIDE_FORCE_ANGLE_MIN)
        {
            // input movement relative to camera.

            var camera_relative_movement = Quaternion.Euler(0, camera_object.transform.eulerAngles.y, 0) * input_directional;

            // camera movement relative to slope.

            var slope_relative_movement = Vector3.ProjectOnPlane(camera_relative_movement, raycast_grounded_slope_normal);

            // project movement relative to plane of slide direction.

            slope_relative_movement = Vector3.ProjectOnPlane(slope_relative_movement, slide_direction);

            //if(is_input_left)
            //{
            //    slope_relative_movement = Vector3.Cross(slide_direction, Vector3.up);
            //}

            //if(is_input_right)
            //{
            //    slope_relative_movement = -Vector3.Cross(slide_direction, Vector3.up);
            //}

            //Debug.DrawRay(this.transform.position, slope_relative_movement, Color.white);

            // force.

            var force = slope_relative_movement * ACCELERATION_AIR;

            // do raycasts.

            is_movement_hit = Physics.SphereCast
                (this.transform.position, GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

            Debug.DrawRay(transform.position, slope_relative_movement, Color.red);

            // apply forces based on raycast hits.

            if (!is_movement_hit)
            {
                    rigid_body.AddForce(force, ForceMode.VelocityChange);
            }
        }
    }

    private void UpdateSlideSlide()
    {
        // recover, or continue sliding
        // based on current situation.

        if (raycast_grounded_slope_angle < SLIDE_ANGLE_RECOVERY_MAX
            && ground_type != GameConstants.GroundType.ground_slide
            && is_spherecast_grounded)
        {
            // increase the slide resistance
            slide_resistance += SLIDE_RESISTANCE_RECOVERY;
        }
        else
        {
            slide_resistance = 0.0f;
        }

        slide_resistance = Mathf.Clamp(slide_resistance, 0.0f, SLIDE_RESISTANCE_MAX);
    }

    private void UpdateSlideSpeed()
    {
        if (rigid_body.velocity.magnitude > MAX_SPEED_SLIDE)
        {
            rigid_body.velocity = Vector3.ClampMagnitude(rigid_body.velocity, MAX_SPEED_GROUNDED);
        }
    }

    #endregion
    #region dive

    private void UpdateDivePlayerState()
    {
        update_count_dive++;

        if(update_count_dive >= UPDATE_COUNT_DIVE_RECOVERY_MIN)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }
    }

    private void UpdateDiveBegin()
    {
        update_count_dive = 0;

        audio_source.clip = master.audio_controller.a_player_dive;
        audio_source.Play();

        if (input_directional.magnitude >= DIVE_MIN_INPUT_DIRECTIONAL_MAGNITUDE)
            dive_direction = Quaternion.Euler(0, camera_object.transform.eulerAngles.y, 0) * input_directional.normalized;
        else
            dive_direction = player_render.transform.forward.normalized;


        // zero out vertical velocity and add diving force.

        rigid_body.velocity = new Vector3
            (rigid_body.velocity.x, 0, rigid_body.velocity.z);

        rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);
        rigid_body.AddForce(dive_direction * (JUMP_FORCE_MULTIPLIER * 2), ForceMode.VelocityChange);
    }

    private void UpdateDiveSpeed()
    {
        Vector3 old_x_z = new Vector3(rigid_body.velocity.x, 0, rigid_body.velocity.z);
        Vector3 old_y = new Vector3(0, rigid_body.velocity.y, 0);

        if (old_x_z.magnitude > MAX_SPEED_DIVE)
        {

            old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_DIVE);
            rigid_body.velocity = old_x_z + old_y;
        }
    }

    #endregion
    #region attack

    private void UpdateAttackPlayerState()
    {
        update_count_attack++;
    }

    private void UpdateAttackBegin()
    {
        update_count_attack = 0;
    }

    #endregion
    #region damage

    private void UpdateDamagePlayerState()
    {
        update_count_damage++;

        if (update_count_damage >= 100)
        {
            ChangePlayerState(PlayerState.player_default);
            return;
        }
    }

    private void UpdateDamageBegin()
    {
        update_count_damage = 0;

        master.player_controller.player_health -= damage_type.damage_amount;

        // zero out velocities.
        rigid_body.velocity = Vector3.zero;

        if (damage_type.damage_direction_type 
            == GameConstants.DamageDirectionType.type_up)
        {
            rigid_body.AddForce(Vector3.up * damage_type.damage_force_multiplier, ForceMode.VelocityChange);
        }
        else if(damage_type.damage_direction_type 
            == GameConstants.DamageDirectionType.type_push)
        {
            rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);
            rigid_body.AddForce((this.transform.position - damage_source.transform.position) * damage_type.damage_force_multiplier, ForceMode.VelocityChange);
        }

        // play sound.

        switch (damage_type.damage_effect_type)
        {
            case GameConstants.DamageEffectType.type_fire:
                audio_source.clip = master.audio_controller.a_player_hurt_fire;
                break;

            default:
                audio_source.clip = master.audio_controller.a_player_hurt_default;
                break;
        }

        audio_source.Play();
    }

    #endregion

    // animator.

    private void UpdateAnimator()
    {
        player_animator.SetInteger("anim_game_state", (int)master.game_state);
        player_animator.SetInteger("anim_player_state", (int)player_state);
        player_animator.SetBool("anim_is_grounded", is_spherecast_grounded);
        player_animator.SetBool("anim_is_moving", rigid_body.velocity.magnitude > 0.2f);
        player_animator.SetFloat("anim_horizontal_speed", is_input_directional ? rigid_body.velocity.magnitude : 0.0f);
        player_animator.SetFloat("anim_vertical_speed", rigid_body.velocity.y);
        player_animator.SetBool("anim_is_input_right", input_directional.x > 0.5);
        player_animator.SetBool("anim_is_input_left", input_directional.x < -0.5);

        // update player facing direction if in valid state.

        if (player_state == PlayerState.player_default
            || player_state == PlayerState.player_water_default
            || player_state == PlayerState.player_water_jump)
        {

            facing_direction = Quaternion.Euler(0,   camera_object.transform.rotation.eulerAngles.y, 0) * input_directional;

            facing_direction_delta = Vector3.RotateTowards(player_render.transform.forward, facing_direction, ANIMATION_TURNING_SPEED_MULTIPLIER, 0.0f);

            // Move our position a step closer to the target.
            player_render.transform.rotation = Quaternion.LookRotation(facing_direction_delta);
        }
        else if(player_state == PlayerState.player_slide)
        {
            facing_direction = new Vector3(slide_direction.x, 0, slide_direction.z);

            facing_direction_delta = Vector3.RotateTowards(player_render.transform.forward, facing_direction, ANIMATION_TURNING_SPEED_MULTIPLIER, 0.0f);

            // Move our position a step closer to the target.
            player_render.transform.rotation = Quaternion.LookRotation(facing_direction_delta);
        }
        else if(player_state == PlayerState.player_dive)
        {
            facing_direction.x = dive_direction.x;
            facing_direction.y = 0;
            facing_direction.z = dive_direction.z;

            facing_direction_delta = Vector3.RotateTowards(player_render.transform.forward, facing_direction, ANIMATION_TURNING_SPEED_MULTIPLIER, 0.0f);

            // Move our position a step closer to the target.
            player_render.transform.rotation = Quaternion.LookRotation(facing_direction_delta);
        }
    }

    private void UpdateAnimatorGameOver()
    {

    }

    private void UpdateAnimatorCutscene()
    {

        if (master.cutscene_controller.event_source == null)
            return;

        facing_direction_delta = Vector3.RotateTowards(player_render.transform.forward,
            master.cutscene_controller.event_source.transform.position - player_render.transform.position,
            ANIMATION_TURNING_SPEED_MULTIPLIER,
            0.0f);

        facing_direction_delta.y = 0.0f;

        // Move our position a step closer to the target.
        player_render.transform.rotation = Quaternion.LookRotation(facing_direction_delta);
    }

    // state change.

    private void ChangeGameState(object sender, EventArgs e)
    {
        GameStateChangeEventArgs args = e as GameStateChangeEventArgs;

        if (args.game_state == GameState.Game)
            player_animator.runtimeAnimatorController = animator_game;
        else if (args.game_state == GameState.GameOver)
            player_animator.runtimeAnimatorController = animator_game_over;
        else if (args.game_state == GameState.Cutscene)
            player_animator.runtimeAnimatorController = animator_cutscene;
        else
            player_animator.runtimeAnimatorController = animator_game;
    }

    private void ChangePlayerState(PlayerState new_state)
    {
        player_state_previous = player_state;
        player_state = new_state;

        if (player_state == PlayerState.player_default)
            UpdateDefaultBegin();
        else if (player_state == PlayerState.player_jump)
            UpdateJumpBegin();
        else if (player_state == PlayerState.player_water_default)
            UpdateWaterDefaultBegin();
        else if (player_state == PlayerState.player_water_jump)
            UpdateWaterJumpBegin();
        else if (player_state == PlayerState.player_slide)
            UpdateSlideBegin();
        else if (player_state == PlayerState.player_dive)
            UpdateDiveBegin();
        else if (player_state == PlayerState.player_attack)
            UpdateAttackBegin();
        else if (player_state == PlayerState.player_damage)
            UpdateDamageBegin();
    }

    private void HandleDamageObject(GameObject damage_object)
    {
        // get the objects damage attributes (or default)
        // the handle moving into the damage state.

        damage_source = damage_object.gameObject;
        damage_type = damage_object.gameObject.GetComponent<ActorAttributeDamageType>()?.damage_type;
        if (damage_type == null)
            damage_type = AttributeDamageTypeData.GetDefault();

        if (player_state != PlayerState.player_damage || damage_type.damage_is_instant)
            ChangePlayerState(PlayerState.player_damage);
    }

    // collision.

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == GameConstants.TAG_MOVING_OBJECT)
        {
            moving_object_collision_list.Add(collision.gameObject);

            is_colliding_moving_object = true;
        }

        if (collision.gameObject.tag == GameConstants.TAG_DAMAGE_OBJECT)
        {
            HandleDamageObject(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == GameConstants.TAG_DAMAGE_OBJECT)
        {
            HandleDamageObject(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(moving_object_collision_list.Contains(collision.gameObject))
        {
            moving_object_collision_list.Remove(collision.gameObject);

            if (moving_object_collision_list.Count == 0)
            {
                is_colliding_moving_object = false;
            }
        }
    }

    // trigger.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_WATER)
        {
            water_object_collision_list.Add(other.gameObject);

            is_colliding_water_object = true;
            water_y_level = other.transform.position.y + (other.bounds.size.y / 2);
        }

        if(other.gameObject.tag == GameConstants.TAG_DAMAGE_OBJECT)
        {
            HandleDamageObject(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_DAMAGE_OBJECT)
        {
            HandleDamageObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (water_object_collision_list.Contains(other.gameObject))
        {
            water_object_collision_list.Remove(other.gameObject);

            if (water_object_collision_list.Count == 0)
            {
                is_colliding_water_object = false;
                water_y_level = 0.0f;
            }
        }
    }

    // interface managers.

    public CameraAudioManager UpdateCameraAudioController()
    {
        if (master.game_state != GameState.Game)
            return null;

        camera_audio_manager.is_submerged = is_full_submerged;
        return camera_audio_manager;
    }

    public ActorFootstepManager UpdateFootstepController()
    {
        if (master.game_state != GameState.Game)
            return null;

        footstep_manager.is_grounded = is_spherecast_grounded;
        footstep_manager.ground_type = ground_type;
        footstep_manager.velocity = rigid_body.velocity.magnitude;
        footstep_manager.is_in_water = is_colliding_water_object;
        footstep_manager.is_submerged = is_full_submerged;
        return footstep_manager;
    }

    public ActorSplashManager UpdateSplashController()
    {
        if (master.game_state != GameState.Game)
            return null;

        splash_manager.water_level = water_y_level;
        splash_manager.is_in_water = is_colliding_water_object;
        splash_manager.is_submerged = is_full_submerged;
        return splash_manager;
    }

    public ActorDamageEffectManager UpdateDamageEffectController()
    {
        if (master.game_state != GameState.Game)
            return null;

        damage_effect_manager.is_active = player_state == PlayerState.player_damage;
        damage_effect_manager.damage_effect_type = damage_type.damage_effect_type;
        damage_effect_manager.actor_renderer = player_renderer;
        return damage_effect_manager;
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(64, Screen.height-600, 600, 600),
            "player_state: " + player_state
            + "\npos " + rigid_body.position.x.ToString("0.00")
            + "|" + rigid_body.position.y.ToString("0.00")
            + "|" + rigid_body.position.z.ToString("0.00")
            + "\nvel " + rigid_body.velocity.x.ToString("0.00")
            + "|" + rigid_body.velocity.y.ToString("0.00")
            + "|" + rigid_body.velocity.z.ToString("0.00")
            + "\nray distance: " + raycast_hit_info.distance
            + "\nray grounded: " + is_raycast_grounded
            + "\nray angle: " + raycast_grounded_slope_angle
            + "\nsphere distance: " + spherecast_hit_info.distance
            + "\nsphere grounded: " + is_spherecast_grounded
            + "\nmoving object collisions: " + moving_object_collision_list.Count
            + "\nis colliding moving object: " + is_colliding_moving_object
            + "\nwater trigger collisions: " + water_object_collision_list.Count
            + "\nis colliding water trigger: " + is_colliding_water_object
            + "\nwater y level: " + water_y_level
            + "\nis partial submerged: " + is_partial_submerged
            + "\nis full submerged: " + is_full_submerged
            + "\nslide resistance: " + slide_resistance);
    }
}
