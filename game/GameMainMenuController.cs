using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMainMenuController : MonoBehaviour
{
    GameMasterController master;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();
        master.input_controller.action_start.performed += LoadTestScene;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void OnDestroy()
    {
        master.input_controller.action_start.performed -= LoadTestScene;
    }

    private void LoadTestScene(InputAction.CallbackContext context)
    {
        Debug.Log("Loading Test Scene");
        master.load_level_controller.StartLoadLevel("scene_test_1", "player_start_1", "camera_start_1");
    }
}
