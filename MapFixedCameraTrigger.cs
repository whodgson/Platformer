using Assets.script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFixedCameraTrigger : MonoBehaviour
{
    public GameObject player_camera;
    public GameObject fixed_transform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            player_camera.GetComponent<CameraController>().SetFixedCamera(fixed_transform.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameConstants.TAG_PLAYER)
        {
            player_camera.GetComponent<CameraController>().UnsetFixedCamera();
        }
    }
}
