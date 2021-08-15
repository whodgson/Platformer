using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class ActorAttributeDamageType : MonoBehaviour
{
    public GameConstants.DamageSourceType damage_source_type = GameConstants.DamageSourceType.type_static;
    public GameConstants.DamageEffectType damage_effect_type = GameConstants.DamageEffectType.type_default;
    public GameConstants.DamageDirectionType damage_direction_type = GameConstants.DamageDirectionType.type_up;
    public int damage_amount = 1;
    public float damage_force_multiplier = 15f;

    public static ActorAttributeDamageType GetDefault()
    {
        return new ActorAttributeDamageType();
    }

}
