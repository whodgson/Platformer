using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using UnityEngine.InputSystem;

public class GamePlayerController : MonoBehaviour
{
    public GameMasterController master;

    private GUIStyle ui_style;
    private GUIStyle ui_shadow_style;

    private Font ui_font;

    private Rect player_lives_rect;
    private Rect player_lives_shadow_rect;

    private Texture2D player_lives_icon_texture;
    private Rect player_lives_icon_rect;

    // player variables.

    public int player_lives = 3;


    // Start is called before the first frame update
    void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();

        ui_font = Resources.Load(GameConstants.DIRECTORY_FONT) as Font;
        
        player_lives_rect = new Rect(54, 20, 32, 32);
        player_lives_shadow_rect = new Rect(56, 22, 32, 32);

        player_lives_icon_texture = Resources.Load("texture/ui/tmenu_player_life") as Texture2D;
        player_lives_icon_rect = new Rect(20, 20, 32, 32);

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

        GUI.DrawTexture(player_lives_icon_rect, player_lives_icon_texture);
        GUI.Label(player_lives_shadow_rect, player_lives.ToString(), ui_shadow_style);
        GUI.Label(player_lives_rect, player_lives.ToString(), ui_style);
    }
}
