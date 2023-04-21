using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGravity : MonoBehaviour
{
    public float forceGravity = 50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * forceGravity);
    }
}
