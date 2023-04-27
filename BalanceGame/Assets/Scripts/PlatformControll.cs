using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformControll : MonoBehaviour
{
    private float time;
    public float maxRot = 60f;
    public float minRot = -60f;


    // Start is called before the first frame update
    void Start()
    {
        time = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        float rotX = transform.localEulerAngles.x;
        float rotZ = transform.localEulerAngles.z;

        if (time > 0)
        {
            time -= Time.deltaTime;
            transform.rotation = Quaternion.identity;
        }
        
        // 최대 기울기 제한
        if (rotX > maxRot)
        {
            rotX = maxRot;
        }
        if (rotX < minRot)
        {
            rotX = minRot;
        }
        if (rotZ > maxRot)
        {
            rotZ = maxRot;
        }
        if (rotZ < minRot)
        {
            rotZ = minRot;
        }



    }
}
