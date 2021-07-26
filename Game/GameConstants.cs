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

        public const string TAG_PLAYER_CAMERA_TARGET = "player_camera_target";
        public const string TAG_MAIN_CAMERA = "MainCamera";

        public const string DIRECTORY_FONT = "font/game_font";

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
            ground_wood
        }

    }

    

}
