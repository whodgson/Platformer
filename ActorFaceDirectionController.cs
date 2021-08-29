using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFaceDirectionController : MonoBehaviour
{
    const float TURNING_SPEED_MULTIPLIER = 0.05f;

    private GameObject target_object = null;

    private Vector3 original_rotation = Vector3.zero;
    private Vector3 new_rotation = Vector3.zero;
    private Vector3 new_rotation_delta = Vector3.zero;

    bool is_active_target = false;
    float unset_lerp = 0.0f;

    public void Start()
    {
        original_rotation = this.transform.rotation.eulerAngles;
    }

    public void Update()
    {
        if (is_active_target)
        {
            if (target_object == null)
                return;

            new_rotation = target_object.transform.position - this.transform.position;
            new_rotation.y = 0.0f;

            new_rotation_delta = Vector3.RotateTowards(this.transform.forward,
                new_rotation,
                TURNING_SPEED_MULTIPLIER,
                0.0f);

            this.transform.rotation = Quaternion.LookRotation(new_rotation_delta);
        }
        else
        {
            if (unset_lerp < 1.0f)
            {
                unset_lerp += Time.deltaTime;
                this.transform.rotation = Quaternion.Euler(Vector3.Lerp(new_rotation, original_rotation, unset_lerp));
            }
        }
    }

    public void SetActive(GameObject new_target_object)
    {
        // start active tracking.

        is_active_target = true;
        target_object = new_target_object;

        // get the new rotation and flatten its
        // y component.

        new_rotation = target_object.transform.position - this.transform.position;
        new_rotation.y = 0.0f;
    }

    public void UnsetActive()
    {
        is_active_target = false;
        new_rotation = this.transform.rotation.eulerAngles;
        unset_lerp = 0.0f;
    }
}
