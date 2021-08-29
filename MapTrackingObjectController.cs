using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrackingObjectController : MonoBehaviour
{
    public GameObject tracking_target;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(tracking_target.transform.position);
    }
}
