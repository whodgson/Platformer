using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class PlayerMovementController : MonoBehaviour
    , IActorFootstepManager
    , ICameraAudioManager
    , IActorSplashManager
{
    // state constants.

    public enum PlayerState
    {
        player_default,
        player_jump,
        player_water_default,
        player_water_jump
    }

    // input constants.

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

    const float JUMP_FORCE_MULTIPLIER = 4.0f;
    const float JUMP_PERSIST_FORCE_MULTIPLIER = 0.4f;
    const int JUMP_PERSIST_ENERGY_MAX = 10;

    // water constants.

    readonly Vector3 WATER_PARTIAL_SUBMERGED_OFFSET = new Vector3(0, 0, 0);
    readonly Vector3 WATER_FULL_SUBMERGED_OFFSET = new Vector3(0, 0.1625f, 0);

    // animation constants.

    const float ANIMATION_TURNING_SPEED_MULTIPLIER = 0.5f;

    // state variables.

    PlayerState player_state = PlayerState.player_default;

    // input variables.

    Vector3 input_directional = Vector3.zero;
    bool is_input_directional = false;
    bool was_input_directional = false;

    bool is_input_jump = false;
    bool was_input_jump = false;

    // component variables.

    private GameMasterController master;
    private Rigidbody rigid_body;
    private SphereCollider player_sphere_collider;
    private Animator player_animator;
    private GameObject camera_object;
    private GameObject player_render;

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

    Vector3 spherecast_grounded_slope_normal = Vector3.up;

    GameConstants.GroundType ground_type;

    // movement variables.

    RaycastHit movement_hit;
    RaycastHit step_movement_hit;

    bool is_movement_hit = false;
    bool is_step_movement_hit = false;

    // jump variables.

    int jump_persist_energy = 0;

    // moving object variables.

    List<GameObject> moving_object_collision_list = new List<GameObject>();
    bool is_colliding_moving_object = false;

    // water variables.

    List<GameObject> water_object_collision_list = new List<GameObject>();
    bool is_colliding_water_object = false;
    bool is_partial_submerged = false;
    bool is_full_submerged = false;
    float water_y_level = 0;

    // interface variables.

    CameraAudioManager camera_audio_manager;
    ActorFootstepManager footstep_manager;
    ActorSplashManager splash_manager;

      
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

        // initialise interface.

        camera_audio_manager = new CameraAudioManager();
        footstep_manager = new ActorFootstepManager();
        splash_manager = new ActorSplashManager();

        // setup.

        InitialisePhysicalParameters();
    }

    private void InitialisePhysicalParameters()
    {
        // rigid body inits.

        rigid_body.mass = RIGID_BODY_MASS;
        rigid_body.drag = RIGID_BODY_DRAG;
        rigid_body.angularDrag = RIGID_BODY_ANGULAR_DRAG;

        rigid_body.constraints = RigidbodyConstraints.FreezeRotation;

        // collider inits.

        grounded_raycast_max_distance = player_sphere_collider.radius + GROUNDED_RAYCAST_ADDITIONAL_DISTANCE;
        grounded_spherecast_max_distance = GROUNDED_SPHERECAST_ADDITIONAL_DISTANCE;
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
                UpdateDefaultWaterPlayerState();

            // do state specific actions.

            if (player_state == PlayerState.player_default)
            {
                UpdateDefaultMovement();
                UpdateDefaultSpeed();
            }

            if(player_state == PlayerState.player_jump)
            {
                UpdateJumpJump();
                UpdateJumpMovement();
                UpdateJumpSpeed();
            }

            if(player_state == PlayerState.player_water_default)
            {
                UpdateDefaultMovement();
                UpdateDefaultWaterSpeed();
            }

            // update animator.

            UpdateAnimator();
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
        // get previous inputs.

        was_input_directional = is_input_directional;
        was_input_jump = is_input_jump;

        // get input from input mapper.

        float input_horizontal = master.input_controller.action_horizontal.ReadValue<float>();
        float input_vertical = master.input_controller.action_vertical.ReadValue<float>();

        Vector3 input = new Vector3(input_horizontal, 0.0f, input_vertical);

        if (input.magnitude > 1)
            input = input.normalized;

        input_directional = input;
        is_input_directional = input_directional.magnitude > INPUT_DIRECTIONAL_THRESHOLD;

        // get button inputs.

        is_input_jump = master.input_controller.action_positive.ReadValue<float>() >= INPUT_BUTTON_THRESHOLD;
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

            // set the slope normal.
            spherecast_grounded_slope_normal = raycast_hit_info.normal;

            // set the grounded type, if grounded.
            if(is_spherecast_grounded)
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
    }

    #region default

    private void UpdateDefaultPlayerState()
    {
        // enter jumping state if right criteria are met.

        if(!was_input_jump && is_input_jump && is_spherecast_grounded)
        {
            player_state = PlayerState.player_jump;
            UpdateJumpBegin();
        }

        if (is_partial_submerged)
            player_state = PlayerState.player_water_default;
    }

    private void UpdateDefaultMovement()
    {
        // input movement relative to camera.

        var camera_relative_movement = Quaternion.Euler(0, camera_object.transform.eulerAngles.y, 0) * input_directional;

        // camera movement relative to slope.

        var slope_relative_movement = Vector3.ProjectOnPlane(camera_relative_movement, raycast_grounded_slope_normal);

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
            is_step_movement_hit = Physics.SphereCast
                (this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out step_movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

            if (!is_step_movement_hit)
            {
                // step obstace, move up and move directly ahead.

                if (rigid_body.velocity.y < STEP_MAX_VELOCITY)
                    rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);

                rigid_body.AddForce(force, ForceMode.VelocityChange);

                // force the sphere grounded status while moving up short steps.
                is_spherecast_grounded = true;
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
        // enter default state if right criteria are met.

        if (rigid_body.velocity.y <= 0)
        {
            player_state = PlayerState.player_default;
        }
    }

    private void UpdateJumpBegin()
    {
        // enter jump state.
        // reset jump power.

        jump_persist_energy = JUMP_PERSIST_ENERGY_MAX;

        // add jumping force.

        rigid_body.velocity = new Vector3
            (rigid_body.velocity.x, 0, rigid_body.velocity.z);

        rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);
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
            is_step_movement_hit = Physics.SphereCast
                (this.transform.position + STEP_MOVEMENT_OFFSET, GROUNDED_SPHERECAST_RADIUS, camera_relative_movement, out step_movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

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

    private void UpdateDefaultWaterPlayerState()
    {
        if (!is_partial_submerged)
            player_state = PlayerState.player_default;
    }

    private void UpdateDefaultWaterSpeed()
    {
        if (rigid_body.velocity.magnitude > MAX_SPEED_WATER)
        {
            rigid_body.velocity = Vector3.ClampMagnitude(rigid_body.velocity, MAX_SPEED_WATER);
        }
    }

    #endregion
    #region jump water

    #endregion

    // animator.

    private void UpdateAnimator()
    {
        player_animator.SetInteger("anim_state", (int)player_state);
        player_animator.SetBool("anim_is_grounded", is_spherecast_grounded);
        player_animator.SetBool("anim_is_moving", rigid_body.velocity.magnitude > 0.2f);
        player_animator.SetFloat("anim_horizontal_speed", is_input_directional ? rigid_body.velocity.magnitude : 0.0f);
        player_animator.SetFloat("anim_vertical_speed", rigid_body.velocity.y);

        // update player facing direction if in valid state.

        if (player_state == PlayerState.player_default)
        {

            Vector3 facing_direction = Quaternion.Euler(0,   camera_object.transform.rotation.eulerAngles.y, 0) * input_directional;

            var facing_direction_delta = Vector3.RotateTowards(player_render.transform.forward, facing_direction, ANIMATION_TURNING_SPEED_MULTIPLIER, 0.0f);

            // Move our position a step closer to the target.
            player_render.transform.rotation = Quaternion.LookRotation(facing_direction_delta);
        }
    }

    // collision.

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == GameConstants.TAG_MOVING_OBJECT)
        {
            moving_object_collision_list.Add(collision.gameObject);

            is_colliding_moving_object = true;
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

    public CameraAudioManager UpdateCameraAudioController()
    {
        camera_audio_manager.is_submerged = is_full_submerged;
        return camera_audio_manager;
    }

    public ActorFootstepManager UpdateFootstepController()
    {
        footstep_manager.is_grounded = is_spherecast_grounded;
        footstep_manager.ground_type = ground_type;
        footstep_manager.velocity = rigid_body.velocity.magnitude;
        footstep_manager.is_in_water = is_colliding_water_object;
        footstep_manager.is_submerged = is_full_submerged;
        return footstep_manager;
    }

    public ActorSplashManager UpdateSplashController()
    {
        splash_manager.water_level = water_y_level;
        splash_manager.is_in_water = is_colliding_water_object;
        splash_manager.is_submerged = is_full_submerged;
        return splash_manager;
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(0, 0, 600, 600),
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
            + "\nis full submerged: " + is_full_submerged);

    }
}
