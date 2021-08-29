using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.script;
using UnityEngine;

namespace Assets.script
{
    class EventSetActorAnimatorController : MonoBehaviour, IEventController
    {
        private GameMasterController master;
        public GameObject actor_object;
        private Animator actor_animator;
        public GameObject next_event_source = null;

        public string trigger = string.Empty;

        void Start()
        {
            master = GameMasterController.GetMasterController();
        }

        public GameObject GetNextEventSource()
        {
            return next_event_source;
        }

        public string GetEventType()
        {
            return GameConstants.EVENT_TYPE_SET_ACTOR_ANIMATOR;
        }

        public void StartEvent()
        {
            if(actor_animator == null)
                actor_animator = actor_object.GetComponentInChildren<Animator>();

            actor_animator.SetTrigger(trigger);
        }

        public void ProcessEvent()
        {
            return;
        }

        public bool FinishEvent()
        {
            return true;
        }
    }
}
