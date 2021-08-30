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
    public class PlayerStateDiveController : IPlayerStateController
    {
        int update_count_dive = 0;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_dive = 0;

            mc.audio_source.clip = mc.master.audio_controller.a_player_dive;
            mc.audio_source.Play();

            if (mc.input_directional.magnitude >= DIVE_MIN_INPUT_DIRECTIONAL_MAGNITUDE)
                mc.dive_direction = Quaternion.Euler(0, mc.camera_object.transform.eulerAngles.y, 0) * mc.input_directional.normalized;
            else
                mc.dive_direction = mc.player_render.transform.forward.normalized;

            // zero out vertical velocity and add diving force.

            mc.rigid_body.velocity = new Vector3
                (mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);

            mc.rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);
            mc.rigid_body.AddForce(mc.dive_direction * (JUMP_FORCE_MULTIPLIER * 2), ForceMode.VelocityChange);
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_dive++;

            if (update_count_dive >= UPDATE_COUNT_DIVE_RECOVERY_MIN)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_default);
                return;
            }
        }

        public void FinishState(PlayerMovementController mc)
        {
            
        }

        public void UpdateState(PlayerMovementController mc)
        {
            UpdateStateSpeed(mc);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            Vector3 old_x_z = new Vector3(mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);
            Vector3 old_y = new Vector3(0, mc.rigid_body.velocity.y, 0);

            if (old_x_z.magnitude > MAX_SPEED_DIVE)
            {

                old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_DIVE);
                mc.rigid_body.velocity = old_x_z + old_y;
            }
        }
    }
}
