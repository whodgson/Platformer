using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneTransitionController : MonoBehaviour
{
    public GameMasterController master;

    // load constants.

    const float LOAD_TIMER_MULTIPLIER = 1;

    // load variables.

    string load_scene_name = string.Empty;
    string load_player_start_transform = string.Empty;
    string load_camera_start_transform = string.Empty;

    float load_timer = 0.0f;

    bool is_loading = false;

    // debug variables.

    Rect debug_label_rectangle = new Rect(0, 0, 600, 200);

    private void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();
        SceneManager.sceneLoaded += EndLoadLevel;
    }

    private void Update()
    {
        if(is_loading)
        {
            load_timer += LOAD_TIMER_MULTIPLIER * Time.deltaTime;

            if(load_timer >= 1.0f)
            {
                DoLoadLevel();
            }
        }
    }

    public void StartLoadLevel(string scene_name, string player_start_transform, string camera_start_transform)
    {
        // Begin loading a level.

        master.ChangeState(GameState.Loading);

        is_loading = true;

        load_timer = 0.0f;

        load_scene_name = scene_name;
        load_player_start_transform = player_start_transform;
        load_camera_start_transform = camera_start_transform;
    }

    private void DoLoadLevel()
    {
        SceneManager.LoadScene(load_scene_name, LoadSceneMode.Single);
    }

    private void EndLoadLevel(Scene scene, LoadSceneMode load_scene_mode)
    {
        // reset after loading.

        master.ChangeState(GameState.Game);

        is_loading = false;

        load_timer = 0.0f;

        load_scene_name = string.Empty;
        load_player_start_transform = string.Empty;
        load_camera_start_transform = string.Empty;

        // initialise the player and camera.


    }

    private void OnGUI()
    {
        if(!Application.isEditor)
            return;

        GUI.Label(debug_label_rectangle, load_timer.ToString());
    }
}
