using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.script
{
    public interface IActorSplashManager
    {
        ActorSplashManager UpdateSplashController();
    }

    public class ActorSplashManager
    {
        public float water_level;
        public bool is_in_water;
        public bool is_submerged;

        public ActorSplashManager()
        {
            water_level = 0f;
            is_in_water = false;
            is_submerged = false;
        }
    }
}
