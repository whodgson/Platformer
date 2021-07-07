using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class MapLoadLevelTrigger : MonoBehaviour
{
    public string scene = string.Empty;
    public string player_start_transform = string.Empty;
    public string camera_start_transform = string.Empty;

    private GameMasterController master_controller;

    private void Start()
    {
        master_controller = GameObject.FindObjectOfType<GameMasterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            master_controller.load_level_controller.StartLoadLevel(scene, player_start_transform, camera_start_transform);
        }
    }
}
