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
    Loading,
    GameOver,
    Cutscene
}

public class GameMasterController : MonoBehaviour
{
    public GameState game_state;
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

    public void ChangeState(GameState new_game_state)
    {
        game_state = new_game_state;
        game_state_change_event_args.game_state = game_state;

        EventHandler handler = GameStateChange;
        if (handler != null) handler(this, game_state_change_event_args);
    }

    public static GameMasterController GetMasterController()
    {
        return GameObject.FindObjectOfType<GameMasterController>();
    }
}

public class GameStateChangeEventArgs : EventArgs
{
    public GameState game_state;
}
