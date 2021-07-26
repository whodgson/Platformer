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

        ActorFootstepManager UpdateFootstepController();
    }

    public class ActorFootstepManager
    {
        public bool is_grounded;
        public GameConstants.GroundType ground_type;
        public float velocity;
        public bool is_in_water;
        public bool is_submerged;

        public ActorFootstepManager()
        {
            is_grounded = false;
            ground_type = GameConstants.GroundType.ground_default;
            velocity = 0f;
            is_in_water = false;
            is_submerged = false;
        }
    }

}
