using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterMainController : MonoBehaviour
{
    #region 정의
    public enum CameraType { FpCamera };
    [Serializable]
    public class Components
    {
        public Camera fpCamera;

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
        [Range(1f, 10f), Tooltip("이동속도")]
        public float speed = 5f;

        [Range(1f, 3f), Tooltip("달리기 이동속도 증가 계수")]
        public float runningCoef = 1.5f;

        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 4.2f;

        [Range(0.0f, 2.0f), Tooltip("점프 쿨타임")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("점프 허용 횟수")]
        public int maxJumpCount = 1;

        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;


        [Range(1f, 75f), Tooltip("등반 가능한 경사각")]
        public float maxSlopeAngle = 50f;

        [Range(0f, 4f), Tooltip("경사로 이동속도 변화율(가속/감속)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;

        [Range(0f, 2f), Tooltip("가속")]
        public float acceleration = 1f;
    }

    [Serializable]
    public class CameraOption
    {
        [Tooltip("게임 시작 시 카메라")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("카메라 상하좌우 회전 속도")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("올려다보기 제한 각도")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("내려다보기 제한 각도")]
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
        [Range(1f, 10f), Tooltip("무게")]
        public float weight = 5f;

        public bool isCurrentFp;
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
    }
    #endregion

    #region 필드, 프로퍼티
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
            Debug.LogError($"{componentName} 컴포넌트를 인스펙터에 넣어주세요");
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
            // 회전은 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Camera
        var allCams = FindObjectsOfType<Camera>();
        foreach (var cam in allCams)
        {
            cam.gameObject.SetActive(false);
        }
        // 설정한 카메라 하나만 활성화
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FpCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);

        TryGetComponent(out CapsuleCollider cCol);
        _groundCheckRadius = cCol ? cCol.radius : 0.1f;
    }

    #endregion

    #region fp movement
    /// 키보드 입력을 통해 필드 초기화
    private void SetValuesByKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) v += 1.0f;
        if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
        if (Input.GetKey(Key.moveRight)) h += 1.0f;

        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.acceleration); // 가속, 감속
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;
        State.isRunning = Input.GetKey(Key.run);
    }

    /// <summary> 1인칭 회전 </summary>
    private void Rotate()
    {
        float deltaCoef = Time.deltaTime * 50f;

        // 상하 : FP Rig 회전
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f){ xRotNext -= 360f; }

        // 좌우 : FP Root 회전
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // 상하 회전 가능 여부
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        // FP Rig 상하 회전 적용
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // FP Root 좌우 회전 적용
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    private void Move()
    {
        // 이동하지 않는 경우, 미끄럼 방지
        if (State.isMoving == false)
        {
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        }

        // 실제 이동 벡터 계산
        _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);

        // Y축 속도는 유지하면서 XZ평면 이동
        Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
    }



    #endregion

    #region jump Methods
 
    /// 땅으로부터의 거리 체크 
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
        if (!State.isGrounded) { return; } // false면 점프 불가

        if (Input.GetKeyDown(Key.jump))
        {
            Com.rBody.AddForce(Vector3.up * MoveOption.jumpForce, ForceMode.VelocityChange);
        }
        
    }

    #endregion

    #region Fallend Methods
    private void DontFall()
    {
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(25, 20, 0);
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
        Jump();
        DontFall();
    }
}
