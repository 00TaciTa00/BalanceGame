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
            // 캡슐과 충돌 시, Material을 검정색 Material로 변경
            _meshRenderer.material = _blackMaterail;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 캡슐에서 떨어지면, Material을 기본 Material로 변경
            _meshRenderer.material = _defaultMaterial;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 검정색 Material 참조
        _blackMaterail = Resources.Load<Material>("Materials/TriggerPlatformMaterial");

        // cylinder 오브젝트의 기본 Material 참조
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
