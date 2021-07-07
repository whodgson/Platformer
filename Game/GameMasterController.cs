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
    public GameLoadLevelController load_level_controller;
    public GameInputController input_controller;
    public GameAudioController audio_controller;
    public GamePlayerController player_controller;
    public GameDataController data_controller;

    // prefabs.

    public GameObject player_prefab;
    public GameObject camera_prefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Cursor.lockState = CursorLockMode.Locked;

        game_state = GameState.MainMenu;

        load_level_controller = this.gameObject.AddComponent<GameLoadLevelController>();
        input_controller = this.gameObject.GetComponent<GameInputController>();
        audio_controller = this.gameObject.GetComponent<GameAudioController>();
        player_controller = this.gameObject.GetComponent<GamePlayerController>();
        data_controller = this.gameObject.GetComponent<GameDataController>();
    }

    public void ChangeState(GameState new_game_state)
    {
        game_state = new_game_state;
    }

    public static GameMasterController GetMasterController()
    {
        return GameObject.FindObjectOfType<GameMasterController>();
    }
}
