using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private Material _defaultMaterial;
    private Material _blackMaterail;
    private MeshRenderer _meshRenderer;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ĸ���� �浹 ��, Material�� ������ Material�� ����
            _meshRenderer.material = _blackMaterail;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ĸ������ ��������, Material�� �⺻ Material�� ����
            _meshRenderer.material = _defaultMaterial;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ������ Material ����
        _blackMaterail = Resources.Load<Material>("Materials/TriggerPlatformMaterial");

        // cylinder ������Ʈ�� �⺻ Material ����
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
