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
    public class PlayerStateSlideController : IPlayerStateController
    {
        int update_count_slide = 0;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_slide = 0;

            mc.slide_force = SLIDE_FORCE_MULIPLIER;
            mc.slide_direction = mc.raycast_grounded_slope_direction;
            mc.rigid_body.AddForce(mc.raycast_grounded_slope_direction * mc.slide_force, ForceMode.VelocityChange);

            mc.audio_source.clip = mc.master.audio_controller.a_player_slide;
            mc.audio_source.Play();
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_slide++;

            // exit if entering water.
            if (mc.is_partial_submerged)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_default);
                return;
            }

            // exit if slide resistance recovered.
            if (mc.slide_resistance >= SLIDE_RESISTANCE_MAX)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_default);
                return;
            }

            // exit if fully in air.
            if (!mc.is_spherecast_grounded && !mc.is_raycast_grounded)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_default);
                return;
            }

            // exit if slow enough to jump.
            if (mc.is_raised_positive
                && mc.is_spherecast_grounded
                && mc.rigid_body.velocity.magnitude < SLIDE_SPEED_RECOVERY_MAX)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_jump);
                return;
            }
        }

        public void FinishState(PlayerMovementController mc)
        {
            
        }

        public void UpdateState(PlayerMovementController mc)
        {
            UpdateStateMovement(mc);
            UpdateStateLateralMovement(mc);
            UpdateStateSlide(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateMovement(PlayerMovementController mc)
        {
            // slide forward.

            if (mc.raycast_grounded_slope_angle >= SLIDE_FORCE_ANGLE_MIN)
            {
                // update the slide vector.

                mc.slide_direction = Vector3.RotateTowards
                    (mc.slide_direction, mc.raycast_grounded_slope_direction.normalized, SLIDE_DIRECTION_ROTATION_MULTIPLIER, 0.0f);

                // add the regular slide force, if no obstacle.

                mc.is_slide_hit = Physics.SphereCast
                    (mc.transform.position, GROUNDED_SPHERECAST_RADIUS, mc.slide_direction, out mc.movement_hit, MOVEMENT_SPHERECAST_DISTANCE);


                if (!mc.is_slide_hit)
                {
                    mc.rigid_body.AddForce(mc.slide_direction * SLIDE_FORCE_MULIPLIER, ForceMode.VelocityChange);
                }
            }
        }

        public void UpdateStateLateralMovement(PlayerMovementController mc)
        {
            // add sideways slide force.

            if (mc.raycast_grounded_slope_angle >= SLIDE_FORCE_ANGLE_MIN)
            {
                // input movement relative to camera.

                var camera_relative_movement = Quaternion.Euler(0, mc.camera_object.transform.eulerAngles.y, 0) * mc.input_directional;

                // camera movement relative to slope.

                var slope_relative_movement = Vector3.ProjectOnPlane(camera_relative_movement, mc.raycast_grounded_slope_normal);

                // project movement relative to plane of slide direction.

                slope_relative_movement = Vector3.ProjectOnPlane(slope_relative_movement, mc.slide_direction);

                // force.

                var force = slope_relative_movement * ACCELERATION_AIR;

                // do raycasts.

                mc.is_movement_hit = Physics.SphereCast
                    (mc.transform.position, GROUNDED_SPHERECAST_RADIUS, slope_relative_movement, out mc.movement_hit, MOVEMENT_SPHERECAST_DISTANCE);

                Debug.DrawRay(mc.transform.position, slope_relative_movement, Color.red);

                // apply forces based on raycast hits.

                if (!mc.is_movement_hit)
                {
                    mc.rigid_body.AddForce(force, ForceMode.VelocityChange);
                }
            }
        }

        public void UpdateStateSlide(PlayerMovementController mc)
        {
            // recover, or continue sliding
            // based on current situation.

            if (mc.raycast_grounded_slope_angle < SLIDE_ANGLE_RECOVERY_MAX
                && mc.ground_type != GameConstants.GroundType.ground_slide
                && mc.is_spherecast_grounded)
            {
                // increase the slide resistance
                mc.slide_resistance += SLIDE_RESISTANCE_RECOVERY;
            }
            else
            {
                mc.slide_resistance = 0.0f;
            }

            mc.slide_resistance = Mathf.Clamp(mc.slide_resistance, 0.0f, SLIDE_RESISTANCE_MAX);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            if (mc.rigid_body.velocity.magnitude > MAX_SPEED_SLIDE)
            {
                mc.rigid_body.velocity = Vector3.ClampMagnitude(mc.rigid_body.velocity, MAX_SPEED_GROUNDED);
            }
        }
    }
}
