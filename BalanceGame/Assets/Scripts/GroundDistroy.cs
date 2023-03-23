using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDistroy : MonoBehaviour
{
    public GameObject platform; // ����� ���� ������Ʈ
    public GameObject statue;
    public GameObject plate;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(platform); // �浹�� ������Ʈ�� Player �±׸� ���� ���, ���� ������Ʈ�� ����
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
