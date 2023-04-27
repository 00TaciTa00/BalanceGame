using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFalling : MonoBehaviour
{
    public GameObject rock1;
    public GameObject rock2;

    private void Start()
    {
        StartCoroutine(SpawnRock());
    }

    private IEnumerator SpawnRock()
    {
        new WaitForSeconds(5f);
        while (true)
        {
            // rock1�� rock2 �� �������� �ϳ��� �����մϴ�.
            GameObject rock = Random.value < 0.5f ? rock1 : rock2;

            // ������ ��ġ�� ȸ������ Rock�� �����մϴ�.
            Instantiate(rock, new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f)), Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));

            // 5�� ���� ����մϴ�.
            yield return new WaitForSeconds(5f);
        }
    }
}
