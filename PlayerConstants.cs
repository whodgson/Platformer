using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.script
{
    public class PlayerConstants
    {
        // input constants.

        public const float INPUT_GAME_STATE_DELAY = 0.5f;
        public const float INPUT_DIRECTIONAL_THRESHOLD = 0.01f;
        public const float INPUT_BUTTON_THRESHOLD = 0.5f;

        // component constants.

        public const string MAIN_COLLIDER_GAME_OBJECT_NAME = "main_collider";
        public const string PLAYER_RENDER_GAME_OBJECT_NAME = "player_render";
        public const string PLAYER_DIRECTION_GAME_OBJECT_NAME = "player_direction";

        // physical constants.

        public const float RIGID_BODY_MASS = 1f;
        public const float RIGID_BODY_DRAG = 1f;
        public const float RIGID_BODY_ANGULAR_DRAG = 0.05f;

        public const float GRAVITY_MULTIPLIER = 2.0f;

        public const float DRAG_GROUNDED = 5f;
        public const float DRAG_AIR = 1f;

        public const float ACCELERATION_GROUNDED = 0.5f;
        public const float ACCELERATION_AIR = 0.2f;

        public const float MAX_SPEED_GROUNDED = 3.0f;
        public const float MAX_SPEED_WATER = 2.0f;
        public const float MAX_SPEED_WATER_SINK = 1.25f;

        public const float MAX_SPEED_SLIDE = 6.0f;
        public const float MAX_SPEED_DIVE = 7.0f;

        // grounded constants.

        public const float GROUNDED_RAYCAST_ADDITIONAL_DISTANCE = 0.1f;
        public const float GROUNDED_SPHERECAST_ADDITIONAL_DISTANCE = 0.025f;

        public const float GROUNDED_RAYCAST_DISTANCE = 100f;

        public const float GROUNDED_SPHERECAST_RADIUS = 0.187f;

        // movement constants.

        public static readonly Vector3 STEP_MOVEMENT_OFFSET = new Vector3(0, 0.15f, 0);
        public const float STEP_MAX_VELOCITY = 1f;

        public const float MOVEMENT_SPHERECAST_DISTANCE = 0.1f;
        public const float MOVEMENT_SPHERECAST_RADIUS = 0.1625f;

        // jump constants.

        public const float JUMP_FORCE_MULTIPLIER = 3.0f;
        public const float JUMP_PERSIST_FORCE_MULTIPLIER = 0.4f;
        public const int JUMP_PERSIST_ENERGY_MAX = 10;

        public const float WATER_JUMP_FORCE_MULTIPLIER = 2.5f;
        public const float MINIMUM_WATER_JUMP_Y_SPEED = -1f;

        public const int UPDATE_COUNT_JUMP_RECOVERY_MIN = 10;

        // slide constants.

        public const float SLIDE_FORCE_ANGLE_MIN = 3f;                         // minimum angle to adjust slide direction to slope.
        public const float SLIDE_ANGLE_RECOVERY_MAX = 30f;                     // maximum angle to recover from slide.
        public const float SLIDE_SPEED_RECOVERY_MAX = 0.25f;                   // maximum speed to recover from slide.
        public const float SLIDE_ANGLE_MIN = 50f;                              // minimum angle to start sliding
        public const float SLIDE_RESISTANCE_GROUND_ANGLE_MULTIPLIER = 0.001f;  // multiplier for ground angle to subtract from resistance.
        public const float SLIDE_RESISTANCE_MAX = 1.0f;                        // maximum slide resistance.
        public const float SLIDE_RESISTANCE_RECOVERY = 0.05f;                  // slide resistance recovery amount
        public const float SLIDE_FORCE_MULIPLIER = 1f;                         // multiplier to slide vector.
        public const float SLIDE_DIRECTION_ROTATION_MULTIPLIER = 0.5f;         // how fast the slide direction matches current slope.

        // dive constants.

        public const float DIVE_MIN_INPUT_DIRECTIONAL_MAGNITUDE = 1.0f;        // minimum input magnitude to influence dive direction.
        public const int UPDATE_COUNT_DIVE_RECOVERY_MIN = 30;                  // minimum update count to recover from dive.

        // water constants.

        public static readonly Vector3 WATER_PARTIAL_SUBMERGED_OFFSET = new Vector3(0, 0, 0);
        public static readonly Vector3 WATER_FULL_SUBMERGED_OFFSET = new Vector3(0, 0.1625f, 0);

        // animation constants.

        public const float ANIMATION_TURNING_SPEED_MULTIPLIER = 0.5f;
    }
}
