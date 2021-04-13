using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Menu,
    Game,
    Loading
}

public class GameMasterController : MonoBehaviour
{
    public GameState game_state;
    public GameSceneTransitionController scene_transition_controller;

    bool is_load_scene = false;
    string load_scene = string.Empty;
    string load_player_start_gameobject = string.Empty;
    string load_camera_start_gameobject = string.Empty;

    // prefabs.

    public GameObject player_prefab;
    public GameObject camera_prefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        game_state = GameState.MainMenu;

        scene_transition_controller = this.gameObject.AddComponent<GameSceneTransitionController>();
    }

    public void ChangeState(GameState new_game_state)
    {
        game_state = new_game_state;
    }
}
