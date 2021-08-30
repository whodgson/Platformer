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
    public class PlayerStateWaterJumpController : IPlayerStateController
    {
        int update_count_water_jump = 0;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_water_jump = 0;

            // enter jump state.
            // reset jump power.

            mc.jump_persist_energy = JUMP_PERSIST_ENERGY_MAX;

            // add jumping force.

            mc.rigid_body.velocity = new Vector3
                (mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);

            mc.rigid_body.AddForce(Vector3.up * WATER_JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);

            // player sound.

            mc.audio_source.clip = mc.master.audio_controller.a_player_water_jump;
            mc.audio_source.Play();
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_water_jump++;

            if (mc.rigid_body.velocity.y <= 0)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_default);
                return;
            }

            if (!mc.is_partial_submerged)
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
            mc.state_jump.UpdateStateJump(mc);
            mc.state_jump.UpdateStateMovement(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            Vector3 old_x_z = new Vector3(mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);
            Vector3 old_y = new Vector3(0, mc.rigid_body.velocity.y, 0);

            if (old_x_z.magnitude > MAX_SPEED_WATER)
            {
                old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_WATER);
            }

            if (old_y.y < -MAX_SPEED_WATER)
            {
                old_y = Vector3.ClampMagnitude(old_y, MAX_SPEED_WATER);
            }

            mc.rigid_body.velocity = old_x_z + old_y;
        }
    }
}
