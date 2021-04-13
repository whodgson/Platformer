using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWaterController : MonoBehaviour
{

    public float scrolling_speed_x = 0.1f;
    public float scrolling_speed_y = 0.1f;
    public float counter_scrolling_speed_x = 0.1f;
    public float counter_scrolling_speed_y = 0.1f;
    Renderer mesh_renderer;

    void Start()
    {
        mesh_renderer = GetComponent<Renderer>();
    }

    
    void Update()
    {
        float offset_x = Time.time * scrolling_speed_x;
        float offset_y = Time.time * scrolling_speed_y;
        float counter_offset_x = Time.time * (counter_scrolling_speed_x * -1);
        float counter_offset_y = Time.time * (counter_scrolling_speed_y * -1);

        mesh_renderer.material.SetTextureOffset("_1Tex", new Vector2(offset_x, offset_y));
        mesh_renderer.material.SetTextureOffset("_2Tex", new Vector2(counter_offset_x, counter_offset_y));

    }
}
