using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainMenuController : MonoBehaviour
{
    GameMasterController master;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.FindObjectOfType<GameMasterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            //master.BeginLoadLevel("scene_test_1", "player_start_1", "camera_start_1");
        }
    }
}
