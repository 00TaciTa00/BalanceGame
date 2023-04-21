using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueAction : MonoBehaviour
{
    public LayerMask whatIsTarget; //추적대상 레이어

    private GameObject player;//추적대상
    private UnityEngine.AI.NavMeshAgent pathFinder; //경로 계산 AI 에이전트

    /*public ParticleSystem hitEffect; //피격 이펙트
    public AudioClip deathSound;//사망 사운드
    public AudioClip hitSound; //피격 사운드
    */

    private Animator enemyAnimator;
    //private AudioSource enemyAudioPlayer; //오디오 소스 컴포넌트

    public float attackDelay = 1f; //공격 딜레이
    private float lastAttackTime; //마지막 공격 시점
    private float dist; //추적대상과의 거리

    public Transform tr;

    private float attackRange = 2.3f;

    //추적 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            //추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (player != null)
            {
                return true;
            }

            //그렇지 않다면 false
            return false;
        }
    }

    private bool canMove;
    private bool canAttack;

    private void Awake()
    {
        //게임 오브젝트에서 사용할 컴포넌트 가져오기
        pathFinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        //enemyAudioPlayer = GetComponent<AudioSource>();
    }

    //적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newSpeed)
    {
        //네비메쉬 에이전트의 이동 속도 설정
        pathFinder.speed = newSpeed;
    }


    void Start()
    {
        //게임 오브젝트 활성화와 동시에 AI의 탐지 루틴 시작
        StartCoroutine(UpdatePath());
        tr = GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        enemyAnimator.SetBool("CanMove", canMove);
        enemyAnimator.SetBool("CanAttack", canAttack);

        if (hasTarget)
        {
            //추적 대상이 존재할 경우 거리 계산은 실시간으로 해야하니 Update()
            dist = Vector3.Distance(tr.position, player.transform.position);
        }
    }

    //추적할 대상의 위치를 주기적으로 찾아 경로 갱신
    private IEnumerator UpdatePath()
    {
        //살아 있는 동안 무한 루프
        while (hasTarget)
        {
            Attack();

            //0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    //추적 대상과의 거리에 따라 공격 실행
    public virtual void Attack()
    {

        //자신이 추적 대상과의 거리가 공격 사거리 안에 있다면
        if (dist < attackRange)
        {
            //공격 반경 안에 있으면 움직임을 멈춘다.
            canMove = false;

            //추적 대상 바라보기
            this.transform.LookAt(player.transform);

            //최근 공격 시점에서 attackDelay 이상 시간이 지나면 공격 가능
            if (lastAttackTime + attackDelay <= Time.time)
            {
                canAttack = true;
            }

            //공격 반경 안에 있지만, 딜레이가 남아있을 경우
            else
            {
                canAttack = false;
            }
        }

        //공격 반경 밖에 있을 경우 추적하기
        else
        {
            canMove = true;
            canAttack = false;
            //계속 추적
            pathFinder.isStopped = false; //계속 이동
            pathFinder.SetDestination(player.transform.position);
        }
    }

    //유니티 애니메이션 이벤트로 휘두를 때 데미지 적용시키기
    public void OnDamageEvent()
    {
        //공격 대상을 지정할 추적 대상의 GameObject 컴포넌트 가져오기
        GameObject attackTarget = player.GetComponent<GameObject>();


        //최근 공격 시간 갱신
        lastAttackTime = Time.time;
    }

}
