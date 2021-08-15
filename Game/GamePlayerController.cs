using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using UnityEngine.InputSystem;

public class GamePlayerController : MonoBehaviour
{
    // ui constants.

    const float X_ORIGIN_HEALTH = 8;
    const float Y_ORIGIN_HEALTH = 8;
    const float X_OFFSET_HEALTH = 50;
    const float Y_OFFSET_HEALTH = 32;

    public GameMasterController master;

    private GUIStyle ui_style;
    private GUIStyle ui_shadow_style;

    private Font ui_font;

    private Texture2D player_health_icon_texture;
    private Rect player_health_icon_rect;

    // player variables.

    public int player_health = 6;
    public int player_max_health = 6;

    // Start is called before the first frame update
    void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();

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
        // TEMP SAVE LOAD
        // TODO REMOVE
        if(Keyboard.current.uKey.wasPressedThisFrame)
        {
            master.data_controller.SaveData();
        }

        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            master.data_controller.LoadData();
        }
    }

    private void OnGUI()
    {
        if (master.game_state != GameState.Game)
            return;

        for(int i = 0; i < player_health; i++)
        {
            player_health_icon_rect.x = X_ORIGIN_HEALTH + (i * X_OFFSET_HEALTH);
            player_health_icon_rect.y = Y_ORIGIN_HEALTH + ((i % 2 == 0) ? 0 : Y_OFFSET_HEALTH);
            GUI.DrawTexture(player_health_icon_rect, player_health_icon_texture);
        }
    }
}
