using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFxSplashController : MonoBehaviour
{
    const float LIFETIME = 1f;
    const float TIMER_MULTIPLIER = 1f;

    float timer;
    float opacity;
    Vector3 scale;
    Material material;
    Color colour;

    public float scale_multiplier = 1f;

    private void Start()
    {
        timer = 0f;
        opacity = 1f;
        scale = new Vector3(0,1,0);
        material = GetComponent<Renderer>().material;
        colour = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        timer += Time.deltaTime * TIMER_MULTIPLIER;
        opacity -= Time.deltaTime * TIMER_MULTIPLIER;

        scale.x = timer * scale_multiplier;
        scale.y = 1f;
        scale.z = timer * scale_multiplier;

        colour.a = opacity;
        material.color = colour;

        this.transform.localScale = scale;

        if (timer >= LIFETIME)
            GameObject.Destroy(this.gameObject);
    }
}
