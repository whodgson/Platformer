using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    public class PlayerEnums
    {
        public enum PlayerState
        {
            player_default,
            player_jump,
            player_water_default,
            player_water_jump,
            player_slide,
            player_dive,
            player_attack,
            player_damage
        }
    }
}
