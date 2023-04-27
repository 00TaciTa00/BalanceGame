using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformControll : MonoBehaviour
{
    private float time;
    public float maxRot = 60f;
    public float minRot = -60f;

    // �ٴ��� ȸ�������� �����ϴ°� ��ǥ
    // x, z : 60 ~ -60 300~360 + 
    // Start is called before the first frame update
    void Start()
    {
        time = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        float rotX = transform.localEulerAngles.x;
        float rotY = transform.localEulerAngles.y;
        float rotZ = transform.localEulerAngles.z;
        print(transform.localEulerAngles);

        if (time > 0)
        {
            time -= Time.deltaTime;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            // �ִ� ���� ����
            if (rotX > maxRot)
            {
                
                //transform.Rotate(maxRot, transform.localEulerAngles.y, transform.localEulerAngles.z, Space.Self);
            }
            if (rotX < minRot)
            {
                
                //transform.Rotate(minRot, transform.localEulerAngles.y, transform.localEulerAngles.z, Space.Self);
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
}
