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
    public class PlayerStateDamageController : IPlayerStateController
    {
        int update_count_damage = 0;

        public void BeginState(PlayerMovementController mc)
        {
            update_count_damage = 0;

            mc.master.player_controller.player_health -= mc.damage_type.damage_amount;

            // zero out velocities.
            mc.rigid_body.velocity = Vector3.zero;

            if (mc.damage_type.damage_direction_type
                == GameConstants.DamageDirectionType.type_up)
            {
                mc.rigid_body.AddForce(Vector3.up * mc.damage_type.damage_force_multiplier, ForceMode.VelocityChange);
            }
            else if (mc.damage_type.damage_direction_type
                == GameConstants.DamageDirectionType.type_push)
            {
                mc.rigid_body.AddForce(Vector3.up * JUMP_FORCE_MULTIPLIER, ForceMode.VelocityChange);
                mc.rigid_body.AddForce((mc.transform.position - mc.damage_source.transform.position) * mc.damage_type.damage_force_multiplier, ForceMode.VelocityChange);
            }

            // play sound.

            switch (mc.damage_type.damage_effect_type)
            {
                case GameConstants.DamageEffectType.type_fire:
                    mc.audio_source.clip = mc.master.audio_controller.a_player_hurt_fire;
                    break;

                default:
                    mc.audio_source.clip = mc.master.audio_controller.a_player_hurt_default;
                    break;
            }

            mc.audio_source.Play();
        }

        public void CheckState(PlayerMovementController mc)
        {
            update_count_damage++;

            if (update_count_damage >= 100)
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
            mc.state_jump.UpdateStateMovement(mc);
        }
    }
}
