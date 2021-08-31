using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.script.PlayerEnums;
using static Assets.script.PlayerConstants;

namespace Assets.script
{
    public class PlayerStateWaterDiveController : IPlayerStateController
    {
        int update_count_water_dive = 0;

        float input_horizontal = 0f;
        float input_vertical = 0f;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_water_dive = 0;

            mc.is_under_gravity = false;

            mc.audio_source.clip = mc.master.audio_controller.a_player_water_jump;
            mc.audio_source.Play();

            mc.dive_direction = mc.player_render.transform.forward.normalized;

            // zero out vertical velocity and add diving force.

            mc.rigid_body.velocity = new Vector3
                (mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);

            mc.rigid_body.AddForce(-Physics.gravity, ForceMode.Acceleration);
            mc.rigid_body.AddForce(mc.dive_direction * (JUMP_FORCE_MULTIPLIER * 2), ForceMode.VelocityChange);
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_water_dive++;

            if (update_count_water_dive >= UPDATE_COUNT_DIVE_RECOVERY_MIN && !mc.is_input_interact)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_default);
                return;
            }

            if (!mc.is_partial_submerged)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_default);
                return;
            }
        }

        public void FinishState(PlayerMovementController mc)
        {
            mc.is_under_gravity = true;
        }

        public void UpdateState(PlayerMovementController mc)
        {
            UpdateStateMovement(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateAnimator(PlayerMovementController mc)
        {
            // update the internal direction transform.

            mc.facing_direction = new Vector3(mc.dive_direction.x, 0, mc.dive_direction.z);

            mc.facing_direction_delta = Vector3.RotateTowards(mc.player_direction.transform.forward, mc.facing_direction, PlayerConstants.ANIMATION_TURNING_SPEED_MULTIPLIER, 0.0f);

            // Move direction transform a step closer to the target.
            mc.player_direction.transform.rotation = Quaternion.LookRotation(mc.facing_direction_delta);

            // tilt the renderer to the swimming direction.

            mc.player_render.transform.rotation = Quaternion.LookRotation(mc.dive_direction);

        }

        public void UpdateStateMovement(PlayerMovementController mc)
        {
            input_horizontal = mc.input_directional.x;
            input_vertical = mc.input_directional.z;

            float horizontal_turning_rate = input_horizontal * 0.1f;
            float vertical_turning_rate = input_vertical * 0.5f;

            mc.dive_direction
                = mc.player_direction.transform.forward
                + mc.player_direction.transform.right * horizontal_turning_rate
                + mc.player_direction.transform.up * vertical_turning_rate;

            mc.rigid_body.AddForce(mc.dive_direction, ForceMode.VelocityChange);
            Debug.DrawRay(mc.transform.position,mc.dive_direction);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            if (mc.rigid_body.velocity.magnitude > MAX_SPEED_WATER)
                mc.rigid_body.velocity = mc.rigid_body.velocity.normalized * MAX_SPEED_WATER;
        }
    }
}
