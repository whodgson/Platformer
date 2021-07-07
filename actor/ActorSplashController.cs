using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class ActorSplashController : MonoBehaviour
{
    const float SPAWN_INTERVAL = 0.25f;
    const float TIMER_MULTIPLIER = 1f;

    public IActorSplashManager manager;
    public GameObject splash_prefab;
    public float splash_scale_multiplier = 1f;

    private float timer;
    private float y_level;
    private bool is_in_water;
    private bool is_submerged;
    private Vector3 spawn_vector;
    private GameObject spawned_splash_prefab;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.gameObject.GetComponent<IActorSplashManager>();

        timer = 0.0f;
        y_level = 0f;
        is_in_water = false;
        is_submerged = false;
        spawn_vector = new Vector3(0,0,0);
        spawned_splash_prefab = null;
    }

    // Update is called once per frame
    void Update()
    {
        (float, bool, bool) data = manager.UpdateSplashController();
        y_level = data.Item1;
        is_in_water = data.Item2;
        is_submerged = data.Item3;


        timer += (Time.deltaTime * TIMER_MULTIPLIER);

        if(timer >= SPAWN_INTERVAL)
        {
            timer = 0.0f;

            

            if (!is_in_water)
                return;

            if (is_submerged)
                return;

            spawn_vector.x = this.transform.position.x;
            spawn_vector.y = y_level;
            spawn_vector.z = this.transform.position.z;

            spawned_splash_prefab = Instantiate(splash_prefab, spawn_vector, Quaternion.identity);
            spawned_splash_prefab.transform.localScale = Vector3.zero;
            spawned_splash_prefab.GetComponent<ActorFxSplashController>().scale_multiplier = splash_scale_multiplier;
        }
    }
}
