using Assets.script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteController : MonoBehaviour
{
    public Camera targetCamera;

    // Start is called before the first frame update
    void Start()
    {
        var camera = GameObject.FindGameObjectWithTag(GameConstants.TAG_MAIN_CAMERA);
        targetCamera = camera.GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(targetCamera.transform);
    }
}
