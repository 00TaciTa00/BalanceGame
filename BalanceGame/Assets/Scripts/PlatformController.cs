using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform capsule; // ĸ�� ������Ʈ�� Transform ������Ʈ
    public float capsuleWeight = 1.0f; // ĸ�� ������Ʈ�� ����

    private Vector3 centerOfMass; // ������ ���� �߽� ��ġ
    private Rigidbody rb; // ������ Rigidbody ������Ʈ

    public float GetDistanceFromCenter(Transform capsule)
    {
        Vector3 distance = capsule.position - transform.position;
        distance.y = 0.0f;

        Debug.Log($"캡슐 발판 거리 :{distance}");
        return distance.magnitude;
    }

    public Vector3 GetCenterOfMass()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 CenterOfMass = new Vector3(bounds.center.x, bounds.min.y + (bounds.size.y * 0.5f), bounds.center.z);
        Debug.Log($"발판 무게 중심 : {CenterOfMass}");
        return CenterOfMass;
    }

    private void UpdateCenterOfMass()
    {
        float distance = GetDistanceFromCenter(capsule); // �÷��̾�� cylinder ������ �Ÿ�

        float x = Mathf.Clamp(centerOfMass.x, -0.5f, 0.5f); // ������ x���� Ŭ�����մϴ�.
        float y = (1 - Mathf.Clamp01(distance / 0.5f)) * 0.5f; // �Ÿ��� ���� y���� ����մϴ�.
        float z = 0f;

        centerOfMass = new Vector3(x, y, z);

        rb.centerOfMass = centerOfMass;

        Debug.Log($"{centerOfMass}, {rb.centerOfMass}"); // test
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        centerOfMass = transform.InverseTransformPoint(capsule.position); // ������ ���� ��ǥ�迡���� ĸ�� ������Ʈ ��ġ
        rb.centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        GetCenterOfMass();
        UpdateCenterOfMass();

        float angle = Mathf.Clamp(centerOfMass.x / capsuleWeight, -30f, 30f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    
}
