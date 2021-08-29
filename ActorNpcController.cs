using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.script.actor 
{
    class ActorNpcController : MonoBehaviour
    {
        private GameMasterController master;
        private GameObject player_object;
        private ActorFaceDirectionController facing_controller;
        public float facing_range = 1.0f;
        

        private bool was_in_range = false;
        private bool is_in_range = false;

        private void Start()
        {
            master = GameMasterController.GetMasterController();
            player_object = GameMasterController.GetPlayerObject();
            facing_controller = GetComponent<ActorFaceDirectionController>();
        }

        private void Update()
        {
            was_in_range = is_in_range;

            is_in_range = Vector3.Distance(this.transform.position, player_object.transform.position) <= facing_range;

            if (!was_in_range && is_in_range)
            {
                facing_controller.SetActive(player_object);
            }
            else if(was_in_range && !is_in_range)
            {
                facing_controller.UnsetActive();
            }
        }
    }
}
