using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueMovement : MonoBehaviour
{
    private Transform target;
    private UnityEngine.AI.NavMeshAgent navAgent;

    public void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Start()
    {
        
        tr = GetComponent<Transform>();

    }

    public void Update()
    {
        navAgent.SetDestination(target.transform.position);
        dist = Vector3.Distance(tr.position, target.transform.position);
        UpdatePath();


        if (canMove == true)
        {
            //navAgent.SetDestination(target.transform.position);
            print("canMove");
        }
        if (canAttack == true)
        {
            print("canAtteck");
        }

    }

    public void LateUpdate()
    {
        print("dist : " + dist);
    }

    //추적할 대상의 위치를 주기적으로 찾아 경로 갱신
    private IEnumerator UpdatePath()
    {
        
        Attack();
        //0.25초 주기로 처리 반복

        yield return new WaitForSeconds(0.25f);
    }

    public float attackDelay = 1f; //공격 딜레이
    private float lastAttackTime; //마지막 공격 시점
    private float dist; //추적대상과의 거리

    public Transform tr;

    public float attackRange = 2.3f; // 공격 거리

    private bool canAttack;
    private bool canMove;

    //추적 대상과의 거리에 따라 공격 실행
    public virtual void Attack()
    {

        //자신이 추적 대상과의 거리가 공격 사거리 안에 있다면
        if (dist < attackRange)
        {

            //공격 반경 안에 있으면 움직임을 멈춘다.
            canMove = false;
            //추적 대상 바라보기
            this.transform.LookAt(target.transform);

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
            canAttack = false;
            canMove = true;
            //계속 추적
            
        }
    }
}
