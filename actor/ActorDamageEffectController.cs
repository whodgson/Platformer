using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class ActorDamageEffectController : MonoBehaviour
{

    const float FLASH_INTERVAL = 0.1f;
    const string EMISSION_MATERIAL_PROPERTY = "_Emission";

    public IActorDamageEffectManager manager;
    private ActorDamageEffectManager manager_data;

    bool was_active = false;
    bool is_active = false;
    GameConstants.DamageEffectType damage_effect_type;
    Renderer actor_renderer;

    float elapsed_time = 0f;
    bool is_flashing = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.gameObject.GetComponent<IActorDamageEffectManager>();
        manager_data = manager.UpdateDamageEffectController();
        actor_renderer = manager_data.actor_renderer;
    }

    // Update is called once per frame
    void Update()
    {
        manager_data = manager.UpdateDamageEffectController();

        was_active = is_active;
        is_active = manager_data.is_active;

        if (!is_active && was_active)
            actor_renderer.material.SetColor(EMISSION_MATERIAL_PROPERTY, Color.black);

        if (!is_active)
            return;

        elapsed_time += Time.deltaTime;
        if (elapsed_time >= FLASH_INTERVAL)
        {
            
            elapsed_time = 0f;
            is_flashing = !is_flashing;
            actor_renderer.material.SetColor(EMISSION_MATERIAL_PROPERTY, is_flashing ? Color.red : Color.black);
        }

    }
}
