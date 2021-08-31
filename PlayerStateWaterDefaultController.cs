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
    public class PlayerStateWaterDefaultController : IPlayerStateController
    {
        int update_count_water_default = 0;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_water_default = 0;
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_water_default++;

            // exit to water dive if pressing interact.

            if (mc.is_raised_interact)
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_dive);
                return;
            }

            // exit to water jump if pressing jump 
            // and grounded, or descending in water.

            if
            (
                mc.is_raised_positive
                && (mc.is_spherecast_grounded || mc.rigid_body.velocity.y <= MINIMUM_WATER_JUMP_Y_SPEED)
            )
            {
                mc.ChangePlayerState(PlayerEnums.PlayerState.player_water_jump);
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
            
        }

        public void UpdateState(PlayerMovementController mc)
        {
            mc.state_default.UpdateStateMovement(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateAnimator(PlayerMovementController mc)
        {
            mc.state_default.UpdateStateAnimator(mc);
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
        {
            Vector3 old_x_z = new Vector3(mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);
            Vector3 old_y = new Vector3(0, mc.rigid_body.velocity.y, 0);

            if (old_x_z.magnitude > MAX_SPEED_WATER)
            {
                old_x_z = Vector3.ClampMagnitude(old_x_z, MAX_SPEED_WATER);
            }

            if (old_y.y < -MAX_SPEED_WATER_SINK)
            {
                //old_y = Vector3.ClampMagnitude(old_y, MAX_SPEED_WATER_SINK);
                old_y.y = -MAX_SPEED_WATER_SINK;
            }

            mc.rigid_body.velocity = old_x_z + old_y;
        }
    }
}
