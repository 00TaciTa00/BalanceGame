using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CharacterMainController : MonoBehaviour
{
    #region ����
    public enum CameraType { FpCamera };
    [Serializable]
    public class Components
    {
        public Camera fpCamera;
        public UnityEngine.AI.NavMeshAgent agent;

        [HideInInspector] public Transform fpRoot;
        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject fpCamObject;
        [HideInInspector] public Rigidbody rBody;
        [HideInInspector] public Animator anim;
    }

    [Serializable]
    public class KeyOption
    {
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode run = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float speed = 5f;

        [Range(1f, 3f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runningCoef = 1.5f;

        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpForce = 0.1f;

        [Range(0.0f, 2.0f), Tooltip("���� ��Ÿ��")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("���� ��� Ƚ��")]
        public int maxJumpCount = 1;

        [Tooltip("�������� üũ�� ���̾� ����")]
        public LayerMask groundLayerMask = -1;


        [Range(1f, 75f), Tooltip("��� ������ ��簢")]
        public float maxSlopeAngle = 100f;

        [Range(0f, 4f), Tooltip("���� �̵��ӵ� ��ȭ��(����/����)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;

        [Range(0f, 2f), Tooltip("����")]
        public float acceleration = 1f;
    }

    [Serializable]
    public class CameraOption
    {
        [Tooltip("���� ���� �� ī�޶�")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("ī�޶� �����¿� ȸ�� �ӵ�")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("�÷��ٺ��� ���� ����")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("�����ٺ��� ���� ����")]
        public float lookDownDegree = 75f;
    }

    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveY = "Move Y";
        public string paramMoveZ = "Move Z";
    }

    [Serializable]
    public class CharacterState
    {
        [Range(1f, 10f), Tooltip("����")]
        public float weight = 1f;

        public bool isCurrentFp;
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
    }
    #endregion

    #region �ʵ�, ������Ƽ
    public Components Com => _components;
    public KeyOption Key => _keyOption;
    public MovementOption MoveOption => _movementOption;
    public CameraOption CamOption => _cameraOption;
    public AnimatorOption AnimOption => _animatorOption;
    public CharacterState State => _state;

    private float _groundCheckRadius;
    private float _distFromGround;

    [SerializeField] private Components _components = new Components();
    [Space]
    [SerializeField] private KeyOption _keyOption = new KeyOption();
    [Space]
    [SerializeField] private MovementOption _movementOption = new MovementOption();
    [Space]
    [SerializeField] private CameraOption _cameraOption = new CameraOption();
    [Space]
    [SerializeField] private AnimatorOption _animatorOption = new AnimatorOption();
    [Space]
    [SerializeField] private CharacterState _state = new CharacterState();

    private Vector3 _moveDir;
    private Vector3 _worldMove;
    private Vector2 _rotation;

    #endregion

    private void Awake()
    {
        InitComponents();
        InitSettings();
    }

    private void LogNotInitializedComponentError<T>(T component, string componentName) where T : Component
    {
        if (component == null)
            Debug.LogError($"{componentName} ������Ʈ�� �ν����Ϳ� �־��ּ���");
    }

    #region Init Methods
    private void InitComponents()
    {
        LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
        TryGetComponent(out Com.rBody);
        Com.anim = GetComponentInChildren<Animator>();

        Com.fpCamObject = Com.fpCamera.gameObject;
        Com.fpRig = Com.fpCamera.transform.parent;
        Com.fpRoot = Com.fpRig.parent;
    }

    private void InitSettings()
    {
        // Rigidbody
        if (Com.rBody)
        {
            // ȸ���� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Camera
        var allCams = FindObjectsOfType<Camera>();
        foreach (var cam in allCams)
        {
            cam.gameObject.SetActive(false);
        }
        // ������ ī�޶� �ϳ��� Ȱ��ȭ
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FpCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);

        TryGetComponent(out CapsuleCollider cCol);
        _groundCheckRadius = cCol ? cCol.radius : 0.1f;
    }

    #endregion

    #region fp movement
    /// Ű���� �Է��� ���� �ʵ� �ʱ�ȭ
    private void SetValuesByKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) v += 1.0f;
        if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
        if (Input.GetKey(Key.moveRight)) h += 1.0f;

        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.acceleration); // ����, ����
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;
        State.isRunning = Input.GetKey(Key.run);
    }

    /// <summary> 1��Ī ȸ�� </summary>
    private void Rotate()
    {
        float deltaCoef = Time.deltaTime * 50f;

        // ���� : FP Rig ȸ��
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f){ xRotNext -= 360f; }

        // �¿� : FP Root ȸ��
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // ���� ȸ�� ���� ����
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        // FP Rig ���� ȸ�� ����
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // FP Root �¿� ȸ�� ����
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    private void Move()
    {
        // �̵����� �ʴ� ���, �̲��� ����
        
        if (State.isMoving == false)
        {
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        } 

        // ���� �̵� ���� ���
        _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);

        // Y�� �ӵ��� �����ϸ鼭 XZ��� �̵�
        Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
        /*
        // WASD �Է� ���� ������ Vector3 ����
        Vector3 move = Vector3.zero;

        // WASD �Է� ���� �޾Ƽ� Vector3 ������ ����
        move.x = Input.GetAxis("Horizontal");
        move.z = Input.GetAxis("Vertical");

        // �̵� ������ NavMesh Agent�� ����
        agent.Move(move * Time.deltaTime);
        */
    }



    #endregion

    #region jump Methods

    /// �����κ����� �Ÿ� üũ 
    private void CheckDistanceFromGround()
    {
        Vector3 ro = transform.position + Vector3.up;
        Vector3 rd = Vector3.down;
        Ray ray = new Ray(ro, rd);

        const float rayDist = 500f;
        const float threshold = 0.01f;

        bool cast =
            Physics.SphereCast(ray, _groundCheckRadius, out var hit, rayDist, MoveOption.groundLayerMask);

        _distFromGround = cast ? (hit.distance - 1.5f + _groundCheckRadius) : float.MaxValue;
        State.isGrounded = _distFromGround <= _groundCheckRadius + threshold;
    }

    private void Jump()
    {
        if (!State.isGrounded) { return; } // false�� ���� �Ұ�

        if (Input.GetKeyDown(Key.jump))
        {
            Com.rBody.AddForce(Vector3.up * MoveOption.jumpForce, ForceMode.VelocityChange);
        }
        
    }

    #endregion

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        SetValuesByKeyInput();
        CheckDistanceFromGround();

        Rotate();
        Move();
        //Jump();
    }
}
