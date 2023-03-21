using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilltedPlatform : MonoBehaviour
{

    public float speed = 1f;
    public float maxTiltAngle = 30f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float tiltAngle = Mathf.Sin(Time.time * speed) * maxTiltAngle;
        Quaternion rotation = Quaternion.Euler(0f, 0f, tiltAngle);
        rb.MoveRotation(rotation);
    }
}
