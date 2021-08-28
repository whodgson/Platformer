using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.script
{
    public interface IEventController
    {
        GameObject GetNextEventSource();
        string GetEventType();
        void StartEvent();
        void ProcessEvent();
        bool FinishEvent();
    }
}
