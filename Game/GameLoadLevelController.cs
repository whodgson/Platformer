using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadLevelController : MonoBehaviour
{
    public GameMasterController master;

    // load constants.

    const float LOAD_TIMER_MULTIPLIER = 1;

    // load variables.

    public string load_scene_name = string.Empty;
    public string load_player_start_transform_name = string.Empty;
    public string load_camera_start_transform_name = string.Empty;

    float load_timer = 0.0f;

    bool is_loading = false;

    // transition variables.

    Rect transition_rectangle = new Rect(0, 0, 9999, 9999);
    Color transition_color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Texture2D transition_texture;

    // debug variables.

    Rect debug_label_rectangle = new Rect(0, 0, 600, 200);

    private void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();
        SceneManager.sceneLoaded += EndLoadLevel;

        // initialise transition variables.

        transition_texture  = new Texture2D(1, 1);
    }

    private void Update()
    {
        if(is_loading)
        {
            // increment the load timer while loading.

            load_timer += LOAD_TIMER_MULTIPLIER * Time.deltaTime;

            if(load_timer >= 1.0f)
            {
                DoLoadLevel();
            }
        }
        else
        {
            // decrement the load timer if no longer loading.

            if(load_timer > 0.0f)
            {
                load_timer -= LOAD_TIMER_MULTIPLIER * Time.deltaTime;
            }
        }
    }

    public void StartLoadLevel(string scene_name, string player_start_transform_name, string camera_start_transform_name)
    {
        // Begin loading a level.

        master.ChangeState(GameState.Loading);

        is_loading = true;

        load_timer = 0.0f;

        load_scene_name = scene_name;
        load_player_start_transform_name = player_start_transform_name;
        load_camera_start_transform_name = camera_start_transform_name;
    }

    private void DoLoadLevel()
    {
        SceneManager.LoadScene(load_scene_name, LoadSceneMode.Single);
    }

    private void EndLoadLevel(Scene scene, LoadSceneMode load_scene_mode)
    {
        // get the player and camera start transforms.

        var player_start_transform = GameObject.Find
            (load_player_start_transform_name).transform;

        var camera_start_transform = GameObject.Find
            (load_camera_start_transform_name).transform;

        // initialise the player and camera.

        var player_prefab = master.player_prefab;
        var camera_prefab = master.camera_prefab;

        var player = Instantiate(player_prefab, 
            player_start_transform.position, 
            player_start_transform.rotation);
        var camera = Instantiate(camera_prefab, 
            camera_start_transform.position, 
            camera_start_transform.rotation);

        // reset after loading.

        master.ChangeState(GameState.Game);

        is_loading = false;

        //load_scene_name = string.Empty;
        //load_player_start_transform_name = string.Empty;
        //load_camera_start_transform_name = string.Empty;
    }

    private void OnGUI()
    {
        if (load_timer > 0.0f)
        {
            transition_color.a = load_timer;
            transition_texture.SetPixel(0, 0, transition_color);
            transition_texture.Apply();
            GUI.DrawTexture(transition_rectangle, transition_texture);
        }

        if (!Application.isEditor)
            return;
    }
}
