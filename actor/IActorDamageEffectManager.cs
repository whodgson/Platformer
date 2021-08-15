using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.script
{
    public interface IActorDamageEffectManager
    {
        ActorDamageEffectManager UpdateDamageEffectController();
    }

    public class ActorDamageEffectManager
    {
        public bool is_active;
        public GameConstants.DamageEffectType damage_effect_type;
        public Renderer actor_renderer;
    }

}
