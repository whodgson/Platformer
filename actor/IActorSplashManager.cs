using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.script
{
    public interface IActorSplashManager
    {
        // movement direction,
        // water level, 
        // in water, 
        // full submerged

        (float, bool, bool) UpdateSplashController();
    }
}
