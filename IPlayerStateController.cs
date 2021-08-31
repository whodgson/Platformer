using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    public interface IPlayerStateController
    {
        void CheckState(PlayerMovementController mc);
        void BeginState(PlayerMovementController mc);
        void UpdateState(PlayerMovementController mc);
        void UpdateStateAnimator(PlayerMovementController mc);
        void FinishState(PlayerMovementController mc);
    }
}
