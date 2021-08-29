using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.script;

public enum GameState
{
    MainMenu,
    Menu,
    Game,
    Loading,
    GameOver,
    Cutscene
}

public class GameMasterController : MonoBehaviour
{
    // state variables.

    public GameState game_state;
    private float game_state_time = 0.0f;

    public float Game_State_Time
    { get => game_state_time; }

    // master components.

    public GameLoadLevelController load_level_controller;
    public GameInputController input_controller;
    public GameAudioController audio_controller;
    public GamePlayerController player_controller;
    public GameDataController data_controller;
    public GameCutsceneController cutscene_controller;
    public GameUserInterfaceController user_interface_controller;

    // event handler

    public event EventHandler GameStateChange;
    private GameStateChangeEventArgs game_state_change_event_args;

    // prefabs.

    public GameObject player_prefab;
    public GameObject camera_prefab;



    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Cursor.lockState = CursorLockMode.Locked;

        game_state = GameState.MainMenu;
        game_state_change_event_args = new GameStateChangeEventArgs();
        game_state_change_event_args.game_state = game_state;

        load_level_controller = this.gameObject.AddComponent<GameLoadLevelController>();
        input_controller = this.gameObject.GetComponent<GameInputController>();
        audio_controller = this.gameObject.GetComponent<GameAudioController>();
        player_controller = this.gameObject.GetComponent<GamePlayerController>();
        data_controller = this.gameObject.GetComponent<GameDataController>();
        cutscene_controller = this.gameObject.GetComponent<GameCutsceneController>();
        user_interface_controller = this.gameObject.GetComponent<GameUserInterfaceController>();
    }

    private void Update()
    {
        game_state_time += Time.deltaTime;
    }

    public void ChangeState(GameState new_game_state)
    {
        game_state = new_game_state;
        game_state_change_event_args.game_state = game_state;
        game_state_time = 0.0f;

        EventHandler handler = GameStateChange;
        if (handler != null) handler(this, game_state_change_event_args);
    }

    public static GameMasterController GetMasterController()
    {
        return GameObject.FindObjectOfType<GameMasterController>();
    }

    public static GameObject GetPlayerObject()
    {
        return GameObject.Find("player");
    }

    public static GameObject GetPlayerCameraObject()
    {
        return GameObject.Find("player_camera");
    }
}

public class GameStateChangeEventArgs : EventArgs
{
    public GameState game_state;
}
