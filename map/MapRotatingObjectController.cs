using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotatingObjectController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 m_EulerAngleVelocity;

    public float x_rotation;
    public float y_rotation;
    public float z_rotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        m_EulerAngleVelocity = new Vector3(x_rotation, y_rotation, z_rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
