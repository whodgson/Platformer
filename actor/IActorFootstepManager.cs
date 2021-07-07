using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    public interface IActorFootstepManager
    {
        // grounded, 
        // ground type, 
        // velocity, 
        // in water, 
        // partial submerged, 

        (bool, string, float, bool, bool) UpdateFootstepController();
    }
}
