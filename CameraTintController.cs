using Assets.script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTintController : MonoBehaviour
{
    bool is_underwater = false;

    Texture2D tint_underwater;
    Color colour_underwater = new Color(0.2f, 0.4f, 1.0f, 0.3f);

    Rect screen_rectangle = new Rect(0, 0, 1920, 1080);//Screen.width, Screen.height);

    private void Start()
    {
        tint_underwater = new Texture2D(1, 1);
        tint_underwater.SetPixel(0, 0, colour_underwater);
        tint_underwater.Apply();


    }

    private void OnGUI()
    {
        if(is_underwater)
            GUI.DrawTexture(screen_rectangle, tint_underwater);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_WATER)
            is_underwater = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameConstants.TAG_WATER)
            is_underwater = false;
    }

}
