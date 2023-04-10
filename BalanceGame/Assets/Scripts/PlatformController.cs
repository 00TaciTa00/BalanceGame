using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 center = transform.position;

        // 충돌 지점과 발판 중심 간의 거리를 계산합니다.
        float distance = Vector3.Distance(collisionPoint, center);
        Debug.Log(distance);

        // 발판을 중심으로 충돌 지점의 방향을 구합니다.
        Vector3 direction = (collisionPoint - center).normalized;

        // 물체의 질량과 발판과 충돌한 위치를 이용해 힘을 계산합니다.
        float forceMagnitude = collision.rigidbody.mass * distance;
        Vector3 force = direction * forceMagnitude;

        // AddForceAtPosition() 메서드를 사용하여 발판을 기울입니다.
        rb.AddForceAtPosition(force, collisionPoint);
    }


}
