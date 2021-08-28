using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using TMPro;
using UnityEngine.UI;
using System;

public class GameUserInterfaceController : MonoBehaviour
{
    // ui constants.

    const float MESSAGE_BOX_FONT_SIZE = 24f;

    // ui variables

    GameMasterController master;
    TMP_FontAsset ui_font;
    GUIStyle ui_style;

    // root.

    GameObject ui_object;
    Canvas ui_canvas;
    CanvasScaler ui_canvas_scaler;

    // cutscene.

    GameObject ui_object_cutscene;
    GameObject ui_message_box;
    RectTransform ui_message_box_rect;
    TextMeshProUGUI ui_message_box_text;

    GameObject ui_message_box_frame;
    Image ui_message_box_frame_image;
    RectTransform ui_message_box_frame_image_rect;
    Sprite ui_message_box_frame_sprite;

    GameObject ui_message_box_icon;
    Image ui_message_box_icon_image;
    RectTransform ui_message_box_icon_rect;
    Sprite ui_message_box_icon_sprite;

    // vox sprites.

    public Dictionary<string,Sprite> vox_sprite_dictionary;

    // ui figures.

    int resolution_x = 0;
    int resolution_y = 0;

    public float ui_x_unit = 1f;
    public float ui_y_unit = 1f;

    private void Awake()
    {
        resolution_x = Screen.width;
        resolution_y = Screen.height;
        AdjustLayout();
    }

    private void Start()
    {
        master = this.GetComponentInParent<GameMasterController>();

        // add event hooks.

        master.GameStateChange += ChangeGameState;

        // load ui resources.

        ui_font = Resources.Load<TMP_FontAsset>("font/game_font");
        ui_message_box_frame_sprite = Resources.Load<Sprite>("texture/ui/tmenu_message_box_image");
        ui_message_box_icon_sprite = Resources.Load<Sprite>("texture/vox/vox_default");

        // initialise and load vox resources.

        vox_sprite_dictionary = new Dictionary<string, Sprite>();

        var vox_sprites = Resources.LoadAll<Sprite>("texture/vox");

        foreach(var vox_sprite in vox_sprites)
        {
            vox_sprite_dictionary.Add(vox_sprite.name, vox_sprite);
        }

        // initialise UI.

        Initialise();
    }

    private void OnDestroy()
    {
        master.GameStateChange -= ChangeGameState;
    }

    void Initialise()
    {
        ui_style = new GUIStyle();

        // create UI.

        ui_object = new GameObject("ui_object");
        ui_object.layer = 5;
        DontDestroyOnLoad(ui_object);

        ui_canvas = ui_object.AddComponent<Canvas>();
        ui_canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        ui_canvas_scaler = ui_object.AddComponent<CanvasScaler>();
        ui_canvas_scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        ui_canvas_scaler.referenceResolution = new Vector2(640, 480);

        ui_object.AddComponent<GraphicRaycaster>();

        // cutscene game objects.

        ui_object_cutscene = new GameObject("ui_object_cutscene");
        ui_object_cutscene.layer = 5;
        ui_object_cutscene.transform.SetParent(ui_object.transform);
        ui_object_cutscene.transform.localPosition = Vector3.zero;

        // frame.

        ui_message_box_frame = new GameObject("ui_message_box_frame");
        ui_message_box_frame.layer = 5;
        ui_message_box_frame.transform.SetParent(ui_object_cutscene.transform);

        ui_message_box_frame_image = ui_message_box_frame.AddComponent<Image>();
        ui_message_box_frame_image.sprite = ui_message_box_frame_sprite;

        ui_message_box_frame_image_rect = ui_message_box_frame_image.GetComponent<RectTransform>();
        ui_message_box_frame_image_rect.localPosition = new Vector3(0, -170, 0);
        ui_message_box_frame_image_rect.sizeDelta = new Vector2(600, 64);

        // icon.

        ui_message_box_icon = new GameObject("ui_message_box_icon");
        ui_message_box_icon.layer = 5;
        ui_message_box_icon.transform.SetParent(ui_object_cutscene.transform);

        ui_message_box_icon_image = ui_message_box_icon.AddComponent<Image>();
        ui_message_box_icon_image.sprite = ui_message_box_icon_sprite;

        ui_message_box_icon_rect = ui_message_box_icon_image.GetComponent<RectTransform>();
        ui_message_box_icon_rect.localPosition = new Vector3(-250, -170, 0);
        ui_message_box_icon_rect.sizeDelta = new Vector2(48, 48);

        // message box.

        ui_message_box = new GameObject("ui_message_box_text");
        ui_message_box.layer = 5;
        ui_message_box.transform.SetParent(ui_object_cutscene.transform);

        ui_message_box_text = ui_message_box.AddComponent<TextMeshProUGUI>();
        ui_message_box_text.color = Color.white;
        ui_message_box_text.font = ui_font;
        ui_message_box_text.fontSize = MESSAGE_BOX_FONT_SIZE;
        ui_message_box_text.text = string.Empty;

        ui_message_box_rect = ui_message_box_text.GetComponent<RectTransform>();
        ui_message_box_rect.localPosition = new Vector3(20, -170, 0);
        ui_message_box_rect.sizeDelta = new Vector2(480, 50);
    }

    void Update()
    {
        if(resolution_x != Screen.width || resolution_y != Screen.height)
        {
            AdjustLayout();
        }

        if(master.game_state == GameState.Cutscene)
        {
            ui_message_box_text.text = master.cutscene_controller.message_box_text;
        }
    }

    void AdjustLayout()
    {
        resolution_x = Screen.width;
        resolution_y = Screen.height;

        ui_x_unit = Screen.width / 100;
        ui_y_unit = Screen.height / 100;
    }

    // state change.

    private void ChangeGameState(object sender, EventArgs e)
    {
        GameStateChangeEventArgs args = e as GameStateChangeEventArgs;

        ui_object_cutscene.SetActive(args.game_state == GameState.Cutscene);
    }

    // message box control.

    public void SetMessageBox(string message_icon)
    {
        ui_message_box_icon_image.sprite = vox_sprite_dictionary[message_icon];
    }

    public void UpdateMessageBox(string message_text)
    {
        ui_message_box_text.text = message_text;
    }

    public void UnsetMessageBox()
    {
        ui_message_box_text.text = string.Empty;
        ui_message_box_icon_sprite = vox_sprite_dictionary["default"];
    }
}
