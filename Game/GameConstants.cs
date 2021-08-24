using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.script
{
    public class GameConstants
    {
        public const string TAG_PLAYER = "Player";
        public const string TAG_WATER =  "water";
        public const string TAG_MOVING_OBJECT = "moving_object";
        public const string TAG_DAMAGE_OBJECT = "damage_object";

        public const string NAME_PLAYER = "player";
        public const string NAME_PLAYER_CAMERA = "player_camera";

        public const string TAG_PLAYER_CAMERA_TARGET = "player_camera_target";
        public const string TAG_MAIN_CAMERA = "MainCamera";

        public const string DIRECTORY_FONT = "font/game_font";

        public const int LAYER_MASK_ONLY_PLAYER = 1 << 8;
        public const int LAYER_MASK_ALL_BUT_PLAYER = ~(1 << 8);

        public enum CameraMode
        {
            camera_default,
            camera_fixed,
            camera_fixed_tracking
        }

        

        public enum DamageSourceType
        {
            type_static,
            type_actor
        }

        public enum DamageEffectType
        {
            type_default,
            type_fire
        }

        public enum DamageDirectionType
        {
            type_up,
            type_down,
            type_push
        }

        public enum ActorTag
        {
            actor_default,
            actor_can_press_switch
        }

        public enum GroundType
        {
            ground_default,
            ground_slide,
            ground_water,
            ground_grass,
            ground_sand,
            ground_stone,
            ground_wood,
            ground_mud,
            ground_metal,
            ground_foliage,
        }

    }

    

}
