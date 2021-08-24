using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePlayerController : MonoBehaviour
{
    // ui constants.

    const float X_ORIGIN_HEALTH = 5;
    const float Y_ORIGIN_HEALTH = 5;
    const float X_OFFSET_HEALTH = 5;
    const float Y_OFFSET_HEALTH = 5;
    const float X_WIDTH_HEALTH = 5;
    const float Y_WIDTH_HEALTH = 5;

    // game over constants.

    const float GAME_OVER_TIMER_INTERVAL = 4f;

    // master.

    public GameMasterController master;
    private GameUserInterfaceController ui;

    // ui variables

    private GUIStyle ui_style;
    private GUIStyle ui_shadow_style;

    private Font ui_font;

    private Texture2D player_health_icon_texture;
    private Rect player_health_icon_rect;

    // game over variables.

    float game_over_timer = 0f;

    // player variables.

    public int player_health = 6;
    public int player_max_health = 6;

    // Start is called before the first frame update
    void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();
        ui = master.user_interface_controller;

        ui_font = Resources.Load(GameConstants.DIRECTORY_FONT) as Font;

        player_health_icon_texture = Resources.Load("texture/ui/tmenu_player_health") as Texture2D;
        player_health_icon_rect = new Rect(64, 64, 64, 64);

        ui_style = new GUIStyle();
        ui_style.font = ui_font;
        ui_style.fontSize = 32;
        ui_style.normal.textColor = Color.white;

        ui_shadow_style = new GUIStyle();
        ui_shadow_style.font = ui_font;
        ui_shadow_style.fontSize = 32;
        ui_shadow_style.normal.textColor = Color.black;

    }

    // Update is called once per frame
    void Update()
    {
        if (master.game_state == GameState.Game)
        {
            // TEMP SAVE LOAD
            // TODO REMOVE
            if (Keyboard.current.uKey.wasPressedThisFrame)
            {
                master.data_controller.SaveData();
            }

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                master.data_controller.LoadData();
            }

            // game over.

            if (player_health <= 0 && master.game_state != GameState.GameOver)
            {
                master.ChangeState(GameState.GameOver);
                game_over_timer = 0f;
            }
        }
        else if(master.game_state == GameState.GameOver)
        {
            game_over_timer += Time.deltaTime;

            if(game_over_timer >= GAME_OVER_TIMER_INTERVAL)
            {
                // TODO USE LOAD LEVEL CONTROLLER.
                foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
                    Destroy(o);
                SceneManager.LoadScene("scene_title");
            }
        }
    }

    private void OnGUI()
    {
        if (master.game_state != GameState.Game)
            return;

        for(int i = 0; i < player_health; i++)
        {
            player_health_icon_rect.x = (X_ORIGIN_HEALTH * ui.ui_x_unit) + ((X_OFFSET_HEALTH * ui.ui_x_unit)  * i);
            player_health_icon_rect.y = (Y_ORIGIN_HEALTH * ui.ui_y_unit);
            player_health_icon_rect.width = (X_WIDTH_HEALTH * ui.ui_x_unit);
            player_health_icon_rect.height = (Y_WIDTH_HEALTH * ui.ui_y_unit);
            GUI.DrawTexture(player_health_icon_rect, player_health_icon_texture);
        }
    }
}
