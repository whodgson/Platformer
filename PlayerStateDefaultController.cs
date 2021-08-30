using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.script;
using static Assets.script.PlayerEnums;
using static Assets.script.PlayerConstants;

namespace Assets.script
{
    public class PlayerStateDefaultController : IPlayerStateController
    {
        int update_count_default = 0;

        // slide constants.

        const float SLIDE_ANGLE_MIN = 50f;                              // minimum angle to start sliding
        const float SLIDE_RESISTANCE_GROUND_ANGLE_MULTIPLIER = 0.001f;  // multiplier for ground angle to subtract from resistance.
        const float SLIDE_RESISTANCE_MAX = 1.0f;                        // maximum slide resistance.

        // variables.

        RaycastHit movement_hit;
        RaycastHit step_movement_hit;

        bool is_movement_hit = false;
        bool is_step_movement_hit = false;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_default = 0;
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_default++;

            // enter jumping state if right criteria are met.

            if (mc.is_raised_positive && mc.is_spherecast_grounded)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_jump);
                return;
            }

            if (mc.is_partial_submerged)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_default);
                return;
            }

            if (mc.slide_resistance <= 0.0f)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_slide);
                return;
            }

            // exit to diving state, if the previous state
            // was jump, attack is pressed, and in air.

            if (mc.is_raised_interact
                && mc.player_state_previous == PlayerEnums.PlayerState.player_jump
                && !mc.is_spherecast_grounded)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_dive);
                return;
            }
        }

        public void FinishState(PlayerMovementController mc)
        {
            return;
        }

        public void UpdateState(PlayerMovementController mc)
        {
            UpdateStateMovement(mc);
            UpdateStateSlide(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateMovement(PlayerMovementController mc)
        {
            // input movement relative to camera.

            var camera_relative_movement = Quaternion.Euler(0, mc.camera_object.transform.eulerAngles.y, 0) * mc.input_directional;

            // camera movement relative to slope.

            var slope_relative_movement = Vector3.ProjectOnPlane(camera_relative_movement, mc.spherecast_grounded_slope_normal);

            // acceleration

            float acceleration = mc.is_spherecast_grounded ? PlayerConstants.ACCELERATION_GROUNDED : PlayerConstants.ACCELERATION_AIR;

            // force

            var force = slope_relative_movement * acceleration;

            // do raycasts.

            is_movement_hit = Physics.SphereCast
                (mc.transform.position, PlayerConstants.GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out movement_hit, PlayerConstants.MOVEMENT_SPHERECAST_DISTANCE);

            Debug.DrawRay(mc.transform.position, slope_relative_movement, Color.red);

            // apply forces based on raycast hits.

            if (is_movement_hit)
            {
                // initial step cast.
                is_step_movement_hit = Physics.SphereCast
                    (mc.transform.position + PlayerConstants.STEP_MOVEMENT_OFFSET, PlayerConstants.GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out step_movement_hit, PlayerConstants.MOVEMENT_SPHERECAST_DISTANCE);

                // addition step check for ceilings.
                if (Physics.CheckSphere(mc.transform.position + PlayerConstants.STEP_MOVEMENT_OFFSET, PlayerConstants.GROUNDED_SPHERECAST_RADIUS, GameConstants.LAYER_MASK_ALL_BUT_PLAYER))
                    is_step_movement_hit = true;

                if (!is_step_movement_hit)
                {
                    // step obstace, move up and move directly ahead.

                    if (mc.rigid_body.velocity.y < PlayerConstants.STEP_MAX_VELOCITY)
                        mc.rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);

                    mc.rigid_body.AddForce(force, ForceMode.VelocityChange);

                    // force the sphere grounded status while moving up short steps.
                    mc.is_spherecast_grounded = true;

                    Debug.DrawRay(mc.transform.position, Vector3.up, Color.magenta);
                }
                else
                {
                    // full obstacle, move on a plane to the collided surface.

                    force = Vector3.ProjectOnPlane(force, movement_hit.normal);
                    mc.rigid_body.AddForce(force, ForceMode.VelocityChange);

                    Debug.DrawRay(mc.transform.position, force, Color.blue);
                }
            }
            else
            {
                // no obstace directly ahead.

                mc.rigid_body.AddForce(force, ForceMode.VelocityChange);

                Debug.DrawRay(mc.transform.position, Vector3.up, Color.green);
            }
        }

        public void UpdateStateSlide(PlayerMovementController mc)
        {
            // move towards the sliding state
            // if the right criteria are met.

            if (mc.raycast_grounded_slope_angle > SLIDE_ANGLE_MIN
                || mc.ground_type == GameConstants.GroundType.ground_slide)
            {
                // reduce the slide resistance
                mc.slide_resistance -= (mc.raycast_grounded_slope_angle * SLIDE_RESISTANCE_GROUND_ANGLE_MULTIPLIER);
            }
            else
            {
                mc.slide_resistance = SLIDE_RESISTANCE_MAX;
            }

            mc.slide_resistance = Mathf.Clamp(mc.slide_resistance, 0.0f, SLIDE_RESISTANCE_MAX);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            // limit speed while in the default state.
            // maximum speed depends on criteria.

            if (mc.is_partial_submerged)
            {
                if (mc.rigid_body.velocity.magnitude > PlayerConstants.MAX_SPEED_WATER)
                {
                    mc.rigid_body.velocity = Vector3.ClampMagnitude(mc.rigid_body.velocity, PlayerConstants.MAX_SPEED_WATER);
                }
            }
            else
            {
                if (mc.is_spherecast_grounded)
                {
                    if (mc.rigid_body.velocity.magnitude > PlayerConstants.MAX_SPEED_GROUNDED)
                    {
                        mc.rigid_body.velocity = Vector3.ClampMagnitude(mc.rigid_body.velocity, PlayerConstants.MAX_SPEED_GROUNDED);
                    }
                }
                else
                {
                    Vector3 old_x_z = new Vector3(mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);
                    Vector3 old_y = new Vector3(0, mc.rigid_body.velocity.y, 0);

                    if (old_x_z.magnitude > PlayerConstants.MAX_SPEED_GROUNDED)
                    {

                        old_x_z = Vector3.ClampMagnitude(old_x_z, PlayerConstants.MAX_SPEED_GROUNDED);
                        mc.rigid_body.velocity = old_x_z + old_y;
                    }
                }
            }
        }
    }
}
