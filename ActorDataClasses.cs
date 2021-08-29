using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.script;

namespace Assets.script
{
    [System.Serializable]
    public class AttributeDamageTypeData
    {
        public GameConstants.DamageSourceType damage_source_type = GameConstants.DamageSourceType.type_static;
        public GameConstants.DamageEffectType damage_effect_type = GameConstants.DamageEffectType.type_default;
        public GameConstants.DamageDirectionType damage_direction_type = GameConstants.DamageDirectionType.type_up;
        public int damage_amount = 1;
        public float damage_force_multiplier = 15f;
        public bool damage_is_instant = false;

        public static AttributeDamageTypeData GetDefault()
        {
            return new AttributeDamageTypeData();
        }
    }
}
