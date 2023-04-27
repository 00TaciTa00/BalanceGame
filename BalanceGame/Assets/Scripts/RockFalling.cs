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
            // rock1과 rock2 중 랜덤으로 하나를 선택합니다.
            GameObject rock = Random.value < 0.5f ? rock1 : rock2;

            // 랜덤한 위치와 회전으로 Rock을 생성합니다.
            Instantiate(rock, new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f)), Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));

            // 5초 동안 대기합니다.
            yield return new WaitForSeconds(5f);
        }
    }
}
