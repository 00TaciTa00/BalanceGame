using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotate : MonoBehaviour
{
    public Transform player;
    private Vector3 playerVector;

    public Vector3 GetVectorFromCenter(Transform player)
    {
        Vector3 vector = player.position - transform.position;
        return vector;
    }

    public IEnumerator RotateTowardsVector(Vector3 targetVector, float rotateTime)
    {
        // 현재 발판의 중심 위치를 기준으로, 매개변수로 받은 벡터 값과의 각도를 구합니다.
        Vector3 from = transform.up;
        Vector3 to = (targetVector - transform.position).normalized;
        float angle = Vector3.SignedAngle(from, to, transform.forward);

        // 회전할 총 각도를 구합니다.
        float totalAngle = Mathf.Abs(angle);

        // 회전시간이 0 이하인 경우, 즉시 회전합니다.
        if (rotateTime <= 0)
        {
            transform.RotateAround(transform.position, transform.forward, angle);
            yield break;
        }

        // 발판을 천천히 회전합니다.
        float elapsedTime = 0;
        while (elapsedTime < rotateTime)
        {
            float t = elapsedTime / rotateTime;
            float currentAngle = totalAngle * t * t * t; // 삼차 함수를 사용해 천천히 회전합니다.
            float deltaAngle = currentAngle - Mathf.Min(totalAngle, currentAngle);
            transform.RotateAround(transform.position, transform.forward, Mathf.Sign(angle) * deltaAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 회전 각도보다 더 회전하지 않도록 보정합니다.
        transform.RotateAround(transform.position, transform.forward, Mathf.Sign(angle) * (totalAngle - Mathf.Min(totalAngle, totalAngle)));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
