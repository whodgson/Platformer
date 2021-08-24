using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;

public class MapCameraModeTrigger : MonoBehaviour
{
    private GameObject player_camera;
    public GameConstants.CameraMode camera_mode;
    public CameraModeChangeData camera_mode_change_data;

    void Start()
    {
        player_camera = GameObject.Find(GameConstants.NAME_PLAYER_CAMERA);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            player_camera.GetComponent<CameraController>().SetCamera(camera_mode, camera_mode_change_data);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            player_camera.GetComponent<CameraController>().UnsetCamera();
        }
    }
}
