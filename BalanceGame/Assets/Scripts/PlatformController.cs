using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform capsule; // 캡슐 오브젝트의 Transform 컴포넌트
    public float weight = 1.0f; // 캡슐 오브젝트의 무게

    private Vector3 centerOfMass; // 발판의 무게 중심 위치
    private Rigidbody rb; // 발판의 Rigidbody 컴포넌트

    public float GetDistanceFromCenter(Transform capsule)
    {
        Vector3 distance = capsule.position - transform.position;
        distance.y = 0f;
        return distance.magnitude;
    }

    private void UpdateCenterOfMass()
    {
        float distance = GetDistanceFromCenter(capsule);

        float x = Mathf.Clamp(centerOfMass.x, -0.5f, 0.5f); // 기존의 x값을 클램핑합니다.
        float y = (1 - Mathf.Clamp01(distance / 0.5f)) * 0.5f; // 거리에 따른 y값을 계산합니다.
        float z = 0f;

        centerOfMass = new Vector3(x, y, z);

        rb.centerOfMass = centerOfMass;

        Debug.Log(centerOfMass); // test
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        centerOfMass = transform.InverseTransformPoint(capsule.position); // 발판의 로컬 좌표계에서의 캡슐 오브젝트 위치
        rb.centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        UpdateCenterOfMass();

        float angle = Mathf.Clamp(centerOfMass.x / weight, -30f, 30f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    
}
