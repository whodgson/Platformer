using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class PlayerMovementController : MonoBehaviour
    ,IActorFootstepManager
    , ICameraAudioManager
    , IActorSplashManager
{
    // input constants.

    const float INPUT_MOVEMENT_THRESHOLD = 0.01f;

    // physics constants.

    const float ACCEL_GROUNDED = 0.5f;
    const float ACCEL_AIR = 0.2f;

    const float DRAG_GROUNDED = 5f;
    const float DRAG_SLIDING = 1f;
    const float DRAG_AIR = 0f;

    const float MAX_GROUND_SPEED = 3.0f;
    const float MAX_WATER_SPEED = 2.0f;

    // collision constants.

    const float SPHERECAST_RADIUS = 0.18f;
    const float SPHERECAST_RAY_DISTANCE = 100f;

    const float RAY_COLLIDER_RAY_DISTANCE = 100f;

    const float GROUNDED_RAY_MAX_DISTANCE = 0.3f;
    const float GROUNDED_SPHERE_MAX_DISTANCE = 0.02f;


    const float STEP_OFFSET_DISTANCE = 0.05f;

    // sliding contstants.

    const float SLIDING_FORCE_ANGLE_MIN = 3f;
    const float SLIDING_ANGLE_MAX_FOR_RECOVERY = 30f;
    const float SLIDING_ANGLE_MIN = 50f;
    const float SLIDING_ANGLE_MAX = 85f;

    const float SLIDING_FORCE_MINIMUM = 4f;
    const float SLIDING_FORCE_MAXIMUM = 16f;
    const float SLIDING_FORCE_MINIMUM_CHANGE = 0.05f;

    const float SLIDING_FORCE_SLOPE_ANGLE_MULTIPLIER = 0.001f;

    const float SLIDING_RESISTANCE_MAX = 1.0f;
    const float SLIDING_RESISTANCE_MINIMUM_CHANGE = 0.01f;
    const float SLIDING_RESISTANCE_SLOPE_ANGLE_MULTIPLIER = 0.01f;

    const float SLIDING_MAX_RECOVERY_SPEED = 1.0f;

    const float SLIDING_MOVEMENT_FORCE_MULTIPLIER = 0.25f;

    // jumping constants.

    const float JUMP_FORCE = 3f;
    const float JUMP_FORCE_POWER = 0.4f;
    const float JUMP_POWER_MAX = 1.0f;

    // swimming constants.

    const float SWIM_FORCE = 0.5f;

    // animation constants.

    const float ANIM_SPEED_FACING_ROTATION = 0.5f;

    // water constants.

    const float WATER_PARTIAL_SUBMERGED_OFFSET = 0.0f;
    const float WATER_FULL_SUBMERGED_OFFSET = 0.1875f;

    // attack constants.

    const float ATTACK_TIMER_INCREMENT = 0.07f;

    // Debug constants.

    private readonly Rect DEBUG_RECTANGLE = new Rect(0, 64, 640, 480);

    // physics variables.

    float movement_upward_slope_similarity = 0.0f;
    float movement_downward_slope_similarity = 0.0f;

    // input variables.

    Vector3 input_movement = new Vector3(0, 0, 0);
    bool is_input_movement = false;
    bool is_input_jumping = false;
    bool is_input_attack = false;

    bool was_input_jumping = false;
    bool was_input_attack = false;

    // collision variables.

    private bool is_grounded_sphere = false;
    private bool is_grounded_ray = false;

    private bool is_sphere_hit = false;
    private bool is_ray_hit = false;

    RaycastHit ray_hit_info;
    RaycastHit sphere_hit_info;

    private float grounded_slope_angle = 0.0f;
    private Vector3 grounded_slope_normal = Vector3.up;
    private Vector3 grounded_slope_direction = Vector3.up;

    string grounded_type = string.Empty;

    private bool is_movement_hit = false;
    private bool is_wall_movement_hit = false;

    // sliding variables.

    private bool was_sliding = false;
    private bool is_sliding = false;
    private float sliding_force = 0.0f;
    private float sliding_resistance;
    private Vector3 sliding_direction;

    // jumping variables.

    private bool is_able_to_jump = false;
    private bool is_jumping = false;
    private float jump_power = 1.0f;

    // component variables.

    public GameObject p_camera;
    private Rigidbody p_rigid_body;
    private SphereCollider p_sphere_collider;
    private Transform p_render;
    private Animator p_animator;

    // audio variables.

    AudioClip audio_p_jump;
    AudioClip audio_p_slide;
    AudioClip audio_p_attack;

    // water variables.

    float water_y_level = 0.0f;
    bool is_in_water = false;
    bool is_partial_submerged = false;
    bool is_full_submerged = false;

    // attack variables.

    bool is_attack = false;
    float attack_timer = 0.0f;

    // facing variables.

    Vector3 facing_direction;
    Vector3 facing_direction_delta;

    // game variables.

    GameMasterController master;

    void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();

        p_rigid_body = GetComponent<Rigidbody>();
        p_sphere_collider = GetComponent<SphereCollider>();
        p_render = this.transform.Find("player_render");
        p_animator = GetComponent<Animator>();
        p_camera = GameObject.FindGameObjectWithTag(GameConstants.TAG_MAIN_CAMERA);

        // load audio.

        audio_p_jump = Resources.Load("sound/sfx_p_jump") as AudioClip;
        audio_p_slide = Resources.Load("sound/sfx_p_slide") as AudioClip;
        audio_p_attack = Resources.Load("sound/sfx_p_attack") as AudioClip;

        // init variables.

        sliding_resistance = SLIDING_RESISTANCE_MAX;
        sliding_direction = Vector3.up;

        facing_direction = Vector3.up;
        facing_direction_delta = Vector3.up;

    }

    

    private void FixedUpdate()
    {
        UpdatePlayerInput();

        UpdatePlayerIsGroundedSphere();
        UpdatePlayerIsGroundedRay();

        UpdatePlayerGravity();
        UpdatePlayerFriction();
        UpdatePlayerDrag();
        UpdatePlayerMovement();
        UpdatePlayerSliding();
        UpdatePlayerWater();
        UpdatePlayerJumping();
        UpdatePlayerSwimming();
        UpdatePlayerAttack();

        UpdatePlayerMovementSpeed();

        UpdatePlayerFacingDirection();
        
        UpdatePlayerAnimationValues();
    }

    private void UpdatePlayerInput()
    {
        float input_horizontal = master.input_controller.action_horizontal.ReadValue<float>();
        float input_vertical = master.input_controller.action_vertical.ReadValue<float>();

        Vector3 input = new Vector3(input_horizontal, 0.0f, input_vertical);

        if (input.magnitude > 1)
            input = input.normalized;

        input_movement = input;
        is_input_movement = (input_movement.magnitude > INPUT_MOVEMENT_THRESHOLD);

        is_input_jumping = master.input_controller.action_positive.ReadValue<float>() > 0.5f;

        is_input_attack = master.input_controller.action_interact.ReadValue<float>() > 0.5f;
    }

    private void UpdatePlayerIsGroundedSphere()
    {
        // General grounded check.

        is_sphere_hit = Physics.SphereCast(this.transform.position, SPHERECAST_RADIUS, Vector3.down, out sphere_hit_info, SPHERECAST_RAY_DISTANCE);

        if(is_sphere_hit)
        {
            // set grounded, if under max distance.
            is_grounded_sphere = sphere_hit_info.distance <= GROUNDED_SPHERE_MAX_DISTANCE;

            // set the slope normal.
            grounded_slope_normal = sphere_hit_info.normal;

            // set the ground slope direction.
            var temp = Vector3.Cross(sphere_hit_info.normal, Vector3.down);
            grounded_slope_direction = Vector3.Cross(temp, sphere_hit_info.normal);

            grounded_type = sphere_hit_info.collider?.sharedMaterial?.name 
                ?? GameConstants.COLLIDER_TYPE_DEFAULT;
        }
        else
        {
            grounded_slope_angle = 0.0f;
            grounded_slope_normal = Vector3.up;
        }
    }

    private void UpdatePlayerIsGroundedRay()
    {
        // Specific triangle grounded check.

        is_ray_hit = Physics.Raycast(this.transform.position, Vector3.down, out ray_hit_info, RAY_COLLIDER_RAY_DISTANCE);

        if(is_ray_hit)
        {
            // set grounded, if under max distance.
            is_grounded_ray = ray_hit_info.distance <= GROUNDED_RAY_MAX_DISTANCE;

            // set the angle of the surface.
            grounded_slope_angle = Vector3.Angle(ray_hit_info.normal, Vector3.up);

            // set the slope normal.
            grounded_slope_normal = ray_hit_info.normal;

            // set the ground slope direction.
            var temp = Vector3.Cross(ray_hit_info.normal, Vector3.down);
            grounded_slope_direction = Vector3.Cross(temp, ray_hit_info.normal);
        }
    }

    private void UpdatePlayerGravity()
    {
        if (p_rigid_body.velocity.y > 0)
        {
            p_rigid_body.AddForce(Physics.gravity, ForceMode.Acceleration);
        }
        else
        {
            p_rigid_body.AddForce(Physics.gravity * 2, ForceMode.Acceleration);
        }
    }

    private void UpdatePlayerFriction()
    {
        p_sphere_collider.material.bounciness = 0;

        if (!is_input_movement && is_grounded_ray && !is_sliding)
        {
            p_sphere_collider.material.dynamicFriction = 100; // 100
            p_sphere_collider.material.staticFriction = 100; // 100
        }
        else
        {
            p_sphere_collider.material.dynamicFriction = 0f;
            p_sphere_collider.material.staticFriction = 0f;
        }
    }

    private void UpdatePlayerDrag()
    {
        if (is_grounded_ray)
        {
            if(is_sliding)
                p_rigid_body.drag = DRAG_SLIDING;
            else
                p_rigid_body.drag = DRAG_GROUNDED;
        }
        else
        {
            p_rigid_body.drag = DRAG_AIR;
        }
    }

    private void UpdatePlayerMovement()
    {
        if (is_attack)
            return;

        // get the input vector with respect to the camera.

        var movement = Quaternion.Euler(0, p_camera.transform.eulerAngles.y, 0) * input_movement;

        // get the movement vector, with respect to the current slope.

        var slope_movement = Vector3.ProjectOnPlane(movement, grounded_slope_normal);

        // get the force for movement.

        float acceleration = (is_grounded_sphere) ? ACCEL_GROUNDED : ACCEL_AIR;

        var force = slope_movement * acceleration;

        // Update the slope similarity.
        // Similarity is determined by the dot products of the normalised vectors.

        movement_upward_slope_similarity = Vector3.Dot(-grounded_slope_direction.normalized, slope_movement.normalized);
        movement_upward_slope_similarity = Mathf.Min(1, movement_upward_slope_similarity);
        movement_upward_slope_similarity = Mathf.Max(0, movement_upward_slope_similarity);

        movement_downward_slope_similarity = Vector3.Dot(grounded_slope_direction.normalized, slope_movement.normalized);
        movement_downward_slope_similarity = Mathf.Min(1, movement_downward_slope_similarity);
        movement_downward_slope_similarity = Mathf.Max(0, movement_downward_slope_similarity);

        // If sliding and grounded, multiply the 
        // force by the movement vs. slope similarity.

        if (is_sliding && is_grounded_sphere)
        {
            force = Vector3.ProjectOnPlane(force, grounded_slope_direction) * SLIDING_MOVEMENT_FORCE_MULTIPLIER;
        }

        // apply the force if no obstacle in the way.

        RaycastHit movement_hit;
        is_movement_hit = Physics.SphereCast(transform.position, SPHERECAST_RADIUS, slope_movement, out movement_hit, 0.01f);

        var step_offset_position = transform.position + (grounded_slope_normal * STEP_OFFSET_DISTANCE);

        RaycastHit step_offset_movement_hit;
        bool is_step_offset_movement_hit = Physics.SphereCast(step_offset_position, SPHERECAST_RADIUS, slope_movement, out step_offset_movement_hit, 0.01f);

        if (!is_movement_hit)
        {
            // without an obstacle, apply the force forward.

            p_rigid_body.AddForce(force, ForceMode.VelocityChange);
        }
        else
        {
            // if there is an obstace, but the offset cast was clear,
            // just apply a small upward force to scale obstacle.

            if (!is_step_offset_movement_hit)
            {
                p_rigid_body.AddForce(force, ForceMode.VelocityChange);
                p_rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);
                return;
            }

            // Get the relative sliding movement
            // against the wall, and apply the force that way.

            var relative_vector = Vector3.ProjectOnPlane(slope_movement, movement_hit.normal);
            var relative_force = relative_vector * acceleration;

            RaycastHit wall_movement_hit;
            is_wall_movement_hit = Physics.SphereCast(transform.position, SPHERECAST_RADIUS, relative_vector, out wall_movement_hit, relative_force.magnitude);

            if(!is_wall_movement_hit)
                p_rigid_body.AddForce(relative_force, ForceMode.VelocityChange);
        }

    }

    private void UpdatePlayerSliding()
    {
        if(is_partial_submerged && is_sliding)
        {
            is_sliding = false;
            sliding_resistance = 1.0f;
            return;
        }

        // Start sliding if in a valid state.

        if((is_grounded_sphere && grounded_type == GameConstants.COLLIDER_TYPE_SLIDE)
            || (is_grounded_sphere && grounded_slope_angle > SLIDING_ANGLE_MIN))
        {
            if (sliding_resistance > 0)
                sliding_resistance -= Mathf.Max(
                    SLIDING_RESISTANCE_MINIMUM_CHANGE, 
                    grounded_slope_angle * SLIDING_RESISTANCE_SLOPE_ANGLE_MULTIPLIER);
        }

        // Decrease slide force if in a valid state.

        if ((is_grounded_sphere && grounded_type != GameConstants.COLLIDER_TYPE_SLIDE)
            && (is_grounded_sphere && grounded_slope_angle <= SLIDING_ANGLE_MAX_FOR_RECOVERY))
        {
            if (sliding_force > 0)
                sliding_force -= SLIDING_FORCE_MINIMUM_CHANGE;

            if (sliding_force < 0)
                sliding_force = 0.0f;
        }

        // Recover from slide if in a valid state.

        if ((is_grounded_sphere && grounded_type != GameConstants.COLLIDER_TYPE_SLIDE) 
            && (is_grounded_sphere && grounded_slope_angle <= SLIDING_ANGLE_MIN))
        {
            if (p_rigid_body.velocity.magnitude < SLIDING_MAX_RECOVERY_SPEED)
            {
                sliding_resistance = 1.0f;
                sliding_force = 0.0f;
                is_sliding = false;
            }
 
        }

        // Cap the sliding resistance.

        if (sliding_resistance < 0.0f)
            sliding_resistance = 0.0f;

        if (sliding_resistance > SLIDING_RESISTANCE_MAX)
            sliding_resistance = SLIDING_RESISTANCE_MAX;

        // Begin sliding, or end sliding, based on the sliding resistance.

        was_sliding = is_sliding;

        // Begin sliding.
        if (!was_sliding && sliding_resistance <= 0.0f)
        {
            is_sliding = true;
            sliding_direction = grounded_slope_direction;
        }

        // Play sound if starting sliding.
        if(!was_sliding && is_sliding)
            AudioSource.PlayClipAtPoint(audio_p_slide, this.transform.position);

        // handle sliding forces, if sliding.

        if(is_sliding)
        {
            if (is_grounded_sphere && grounded_slope_angle > SLIDING_FORCE_ANGLE_MIN)
                sliding_direction = Vector3.RotateTowards(sliding_direction, grounded_slope_direction.normalized, 0.5f, 0.0f);

            // increment and apply sliding forces if in the right criteria.

            if ((is_grounded_sphere && grounded_type == GameConstants.COLLIDER_TYPE_SLIDE)
                || (is_grounded_sphere && grounded_slope_angle > SLIDING_ANGLE_MIN))
            {
                if (sliding_force < SLIDING_FORCE_MINIMUM)
                    sliding_force = SLIDING_FORCE_MINIMUM;

                sliding_force += Mathf.Max(
                    SLIDING_FORCE_MINIMUM_CHANGE,
                    grounded_slope_angle * SLIDING_FORCE_SLOPE_ANGLE_MULTIPLIER);

                if (sliding_force > SLIDING_FORCE_MAXIMUM)
                    sliding_force = SLIDING_FORCE_MAXIMUM;

                p_rigid_body.AddForce(sliding_direction * sliding_force, ForceMode.Acceleration);
            }
        }
    }

    private void UpdatePlayerWater()
    {
        // Don't do anything if not in water.

        if (!is_in_water)
            return;

        // Set the water flags.

        is_partial_submerged = (this.transform.position.y + WATER_PARTIAL_SUBMERGED_OFFSET < water_y_level);
        is_full_submerged = (this.transform.position.y + WATER_FULL_SUBMERGED_OFFSET < water_y_level);
    }

    private void UpdatePlayerJumping()
    {
        // Cancel the jump flag if descending.
        if (p_rigid_body.velocity.y <= 0)
            is_jumping = false;

        // Cancel jumping if underwater.
        if (is_full_submerged)
            return;

        // Check if able to jump.
        is_able_to_jump = ((is_input_jumping && !was_input_jumping && is_grounded_sphere && !is_jumping) 
            || (is_input_jumping && !is_grounded_sphere && is_partial_submerged && !is_jumping && p_rigid_body.velocity.y > 0f));

        if (is_able_to_jump)
        {
            // Set jump flag.

            is_jumping = true;

            // Play the jump sound.

            AudioSource.PlayClipAtPoint(audio_p_jump, this.transform.position);

            // Set the jump vector.

            Vector3 jump_direction = (is_sliding) ? grounded_slope_normal : Vector3.up;

            // Starting a new jump, set the jump power back to full.

            jump_power = JUMP_POWER_MAX;

            // Zero out the y velocity before adding the jump force.
            // This prevents bouncing.

            if(is_sliding)
                p_rigid_body.velocity = new Vector3(0, 0, 0);
            else
                p_rigid_body.velocity = new Vector3(p_rigid_body.velocity.x, 0, p_rigid_body.velocity.z);

            // Apply the jump force.

            p_rigid_body.AddForce(jump_direction * JUMP_FORCE, ForceMode.VelocityChange);
        }

        // if airborne and still on the upward arc, add jump power
        // as long as the button is pressed and there is jump power
        // available to use.
        // If the jump button is released at any point, the jump power
        // is immediately set to 0.

        if (!is_input_jumping)
            jump_power = 0;

        if (is_input_jumping && !is_grounded_sphere && p_rigid_body.velocity.y > 0)
        {
            // Set the jump vector.

            Vector3 jump_direction = (is_sliding) ? grounded_slope_normal : Vector3.up;

            if (jump_power > 0)
            {
                p_rigid_body.AddForce(jump_direction * JUMP_FORCE_POWER, ForceMode.VelocityChange);
                jump_power -= 0.1f;
            }
        }

        // update the was-jumping flag, for edge detection.

        was_input_jumping = is_input_jumping;
    }

    private void UpdatePlayerSwimming()
    {
        if (!is_full_submerged)
            return;

        if(is_input_jumping)
        {
            p_rigid_body.AddForce(Vector3.up * SWIM_FORCE, ForceMode.VelocityChange);
        }
    }

    private void UpdatePlayerAttack()
    {
        // exit attack if not in valid state.
        if(is_full_submerged)
        {
            is_attack = false;
            attack_timer = 0.0f;
        }

        // begin attack if in a valid state and not already attacking.
        if (is_input_attack && !was_input_attack && !is_attack && !is_full_submerged)
        {
            is_attack = true;
            attack_timer = 0.0f;

            // Play the attack sound.

            AudioSource.PlayClipAtPoint(audio_p_attack, this.transform.position);
        }

        // if attacking, run attack timer to complete attack.

        if(is_attack)
        {
            attack_timer += ATTACK_TIMER_INCREMENT;

            if (attack_timer >= 1)
                is_attack = false;
        }

        // update the was-attacking flag, for edge detection.

        was_input_attack = is_input_attack;
    }

    private void UpdatePlayerMovementSpeed()
    {
        if (is_partial_submerged)
        {
            if (p_rigid_body.velocity.magnitude > MAX_WATER_SPEED)
            {
                p_rigid_body.velocity = Vector3.ClampMagnitude(p_rigid_body.velocity, MAX_WATER_SPEED);
            }

            return;
        }

        if (is_grounded_sphere)
        {
            if (p_rigid_body.velocity.magnitude > MAX_GROUND_SPEED)
            {
                p_rigid_body.velocity = Vector3.ClampMagnitude(p_rigid_body.velocity, MAX_GROUND_SPEED);
            }
        }
        else
        {
            Vector3 old_x_z = new Vector3(p_rigid_body.velocity.x, 0, p_rigid_body.velocity.z);
            Vector3 old_y = new Vector3(0,p_rigid_body.velocity.y,0);

            if(old_x_z.magnitude > MAX_GROUND_SPEED)
            {
                
                old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_GROUND_SPEED);
                p_rigid_body.velocity = old_x_z + old_y;
            }
        }
    }

    private void UpdatePlayerFacingDirection()
    {
        // don't update rotation if in not in a valid state.

        if ((!is_grounded_sphere && !is_full_submerged)
            || (is_attack))
            return;

        if (is_sliding)
            facing_direction = Vector3.ProjectOnPlane(sliding_direction, Vector3.up);
        else
            facing_direction = Quaternion.Euler(0, p_camera.transform.eulerAngles.y, 0) * input_movement;

        facing_direction_delta = Vector3.RotateTowards(p_render.forward, facing_direction, ANIM_SPEED_FACING_ROTATION, 0.0f);

        // Move our position a step closer to the target.
        p_render.rotation = Quaternion.LookRotation(facing_direction_delta);
    }

    


    private void UpdatePlayerAnimationValues()
    {
        p_animator.SetBool("anim_is_grounded", is_grounded_ray);
        p_animator.SetBool("anim_is_moving", p_rigid_body.velocity.magnitude > 0.025f);
        p_animator.SetBool("anim_is_rising", p_rigid_body.velocity.y > 0);
        p_animator.SetBool("anim_is_sliding", is_sliding);
        p_animator.SetBool("anim_is_full_submerged", is_full_submerged);
        p_animator.SetBool("anim_is_attack", is_attack);
        p_animator.SetFloat("anim_moving_speed", p_rigid_body.velocity.magnitude);
        
    }

    // Trigger behaviour.

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_WATER)
        {
            is_in_water = true;
            water_y_level = other.transform.position.y + (other.bounds.size.y / 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameConstants.TAG_WATER)
        {
            is_in_water = false;
            is_partial_submerged = false;
            is_full_submerged = false;
            water_y_level = 0.0f;
        }
    }

    // Foostep interface.

    public (bool, string, float, bool, bool) UpdateFootstepController()
    {
        if (is_attack || is_sliding)
        {
            return (false,
                grounded_type,
                p_rigid_body.velocity.magnitude,
                is_in_water,
                is_partial_submerged);
        }

        return (is_grounded_ray, 
            grounded_type, 
            p_rigid_body.velocity.magnitude, 
            is_in_water, is_partial_submerged);
    }

    // Camera audio interface.

    public bool UpdateCameraAudioController()
    {
        return is_full_submerged;
    }

    // Splash interface.

    public (float, bool, bool) UpdateSplashController()
    {
        return (water_y_level, is_in_water, is_full_submerged);
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(DEBUG_RECTANGLE,
            "movement hit: " + is_movement_hit.ToString()
            + "\nupward slope similarity: " + movement_upward_slope_similarity.ToString()
            + "\ndownward slope similarity: " + movement_downward_slope_similarity.ToString()
            + "\nsliding force: " + sliding_force.ToString()
            + "\nfloor angle: " + grounded_slope_angle
            + "\nsliding resistance: " + sliding_resistance
            + "\nis sliding: " + is_sliding
            + "\ngrounded sphere: " + is_grounded_sphere
            + "\ngrounded ray: " + is_grounded_ray
            +"\nfriction: " + p_sphere_collider.material.dynamicFriction
            +"\ndrag: " + p_rigid_body.drag);
    }
}
