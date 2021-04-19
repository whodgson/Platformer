using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    public class GameConstants
    {
        public const string TAG_PLAYER = "Player";
        public const string TAG_WATER =  "water";
        public const string TAG_PLAYER_CAMERA_TARGET = "player_camera_target";
        public const string TAG_MAIN_CAMERA = "MainCamera";

        public const string COLLIDER_TYPE_DEFAULT = "c_default";
        public const string COLLIDER_TYPE_WATER = "c_water";
        public const string COLLIDER_TYPE_STONE = "c_stone";
        public const string COLLIDER_TYPE_SAND = "c_sand";
        public const string COLLIDER_TYPE_GRASS = "c_grass";
        public const string COLLIDER_TYPE_WOOD = "c_wood";

        public enum ActorTag
        {
            actor_default,
            actor_can_press_switch
        }
    }

    

}
