using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    interface IMapTriggerReceiver
    {
        void OnActivate();
        void OnDeactivate();
    }
}
