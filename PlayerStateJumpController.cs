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
    public class PlayerStateJumpController : IPlayerStateController
    {
        // variables.

        int update_count_jump = 0;

        RaycastHit movement_hit;
        RaycastHit step_movement_hit;

        bool is_movement_hit = false;
        bool is_step_movement_hit = false;

        public void BeginState(PlayerMovementController mc)
        {
            // reset the update count.

            update_count_jump = 0;

            // if coming from the water jump state,
            // don't add any additional force.

            if (mc.player_state_previous == PlayerState.player_water_jump)
                return;

            // enter jump state.
            // reset jump power.

            mc.jump_persist_energy = PlayerConstants.JUMP_PERSIST_ENERGY_MAX;

            // add jumping force.

            mc.rigid_body.velocity = new Vector3
                (mc.rigid_body.velocity.x, 0, mc.rigid_body.velocity.z);

            mc.rigid_body.AddForce(Vector3.up * PlayerConstants.JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);

            // player sound.

            mc.audio_source.clip = mc.master.audio_controller.a_player_jump;
            mc.audio_source.Play();
        }

        public void CheckState(PlayerMovementController mc)
        {
            // increment the update count.

            update_count_jump++;

            // enter default state if right criteria are met.

            if (mc.rigid_body.velocity.y <= 0)
            {
                mc.ChangePlayerState(PlayerState.player_default);
                return;
            }

            // enter default state if grounded.

            if (update_count_jump >= PlayerConstants.UPDATE_COUNT_JUMP_RECOVERY_MIN
                && (mc.is_raycast_grounded || mc.is_spherecast_grounded))
            {
                mc.ChangePlayerState(PlayerState.player_default);
                return;
            }

            // enter dive state.

            if (mc.is_raised_interact)
            {
                mc.ChangePlayerState(PlayerState.player_dive);
                return;
            }
        }

        public void FinishState(PlayerMovementController mc)
        {
            return;
        }

        public void UpdateState(PlayerMovementController mc)
        {
            UpdateStateJump(mc);
            UpdateStateMovement(mc);
            UpdateStateSpeed(mc);
        }

        public void UpdateStateAnimator(PlayerMovementController mc)
        {
        }

        public void UpdateStateJump(PlayerMovementController mc)
        {
            // decrement the remaining jump persist energy.

            mc.jump_persist_energy -= 1;

            if (mc.is_input_positive && mc.jump_persist_energy > 0)
            {
                // if the jump input is given, and persist energy > 0, add extra jump force.

                mc.rigid_body.AddForce(Vector3.up * PlayerConstants.JUMP_PERSIST_FORCE_MULTIPLIER, ForceMode.VelocityChange);
            }
            else
            {
                // if the jump input is let go, zero out the jump persist energy.

                mc.jump_persist_energy = 0;
            }
        }

        public void UpdateStateMovement(PlayerMovementController mc)
        {
            // input movement relative to camera.

            var camera_relative_movement = Quaternion.Euler(0, mc.camera_object.transform.eulerAngles.y, 0) * mc.input_directional;

            // force

            var force = camera_relative_movement * PlayerConstants.ACCELERATION_AIR;

            // do raycasts.

            is_movement_hit = Physics.SphereCast(
                mc.transform.position, 
                PlayerConstants.GROUNDED_SPHERECAST_RADIUS, 
                camera_relative_movement, 
                out movement_hit, 
                PlayerConstants.MOVEMENT_SPHERECAST_DISTANCE);

            // apply forces based on raycast hits.

            if (is_movement_hit)
            {
                // initial step cast.
                is_step_movement_hit = Physics.SphereCast(
                    mc.transform.position + PlayerConstants.STEP_MOVEMENT_OFFSET, 
                    PlayerConstants.GROUNDED_SPHERECAST_RADIUS, 
                    camera_relative_movement, 
                    out step_movement_hit, 
                    PlayerConstants.MOVEMENT_SPHERECAST_DISTANCE);

                // second step check for ceilings.
                if (Physics.CheckSphere(
                    mc.transform.position + PlayerConstants.STEP_MOVEMENT_OFFSET, 
                    PlayerConstants.GROUNDED_SPHERECAST_RADIUS, 
                    GameConstants.LAYER_MASK_ALL_BUT_PLAYER))
                    is_step_movement_hit = true;

                if (!is_step_movement_hit)
                {
                    // step obstace, move up and move directly ahead.

                    if (mc.rigid_body.velocity.y < PlayerConstants.STEP_MAX_VELOCITY)
                        mc.rigid_body.AddForce(Vector3.up, ForceMode.VelocityChange);

                    mc.rigid_body.AddForce(force, ForceMode.VelocityChange);
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
            }
        }

        public void UpdateStateSpeed(PlayerMovementController mc)
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
