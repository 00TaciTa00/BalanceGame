using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDistroy : MonoBehaviour
{
    public GameObject platform; // 사라질 발판 오브젝트
    public GameObject statue;
    public GameObject plate;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(platform); // 충돌한 오브젝트가 Player 태그를 가진 경우, 발판 오브젝트를 삭제
            Destroy(statue);
            Destroy(plate);

            other.transform.position = new Vector3(8, other.transform.position.y, other.transform.position.z);
        }
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
