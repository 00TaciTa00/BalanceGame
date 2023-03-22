using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorMass : MonoBehaviour
{

    public void DebugLogCenterOfMass()
    {
        Vector3 centerOfMass = GetComponent<Rigidbody>().centerOfMass;
        Debug.LogFormat("Center of Mass1: ({0}, {1}, {2})", centerOfMass.x, centerOfMass.y, centerOfMass.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugLogCenterOfMass();
    }
}
