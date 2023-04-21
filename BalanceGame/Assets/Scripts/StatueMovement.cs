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

    //������ ����� ��ġ�� �ֱ������� ã�� ��� ����
    private IEnumerator UpdatePath()
    {
        
        Attack();
        //0.25�� �ֱ�� ó�� �ݺ�

        yield return new WaitForSeconds(0.25f);
    }

    public float attackDelay = 1f; //���� ������
    private float lastAttackTime; //������ ���� ����
    private float dist; //���������� �Ÿ�

    public Transform tr;

    public float attackRange = 2.3f; // ���� �Ÿ�

    private bool canAttack;
    private bool canMove;

    //���� ������ �Ÿ��� ���� ���� ����
    public virtual void Attack()
    {

        //�ڽ��� ���� ������ �Ÿ��� ���� ��Ÿ� �ȿ� �ִٸ�
        if (dist < attackRange)
        {

            //���� �ݰ� �ȿ� ������ �������� �����.
            canMove = false;
            //���� ��� �ٶ󺸱�
            this.transform.LookAt(target.transform);

            //�ֱ� ���� �������� attackDelay �̻� �ð��� ������ ���� ����
            if (lastAttackTime + attackDelay <= Time.time)
            {
                canAttack = true;
            }

            //���� �ݰ� �ȿ� ������, �����̰� �������� ���
            else
            {
                canAttack = false;
            }
        }

        //���� �ݰ� �ۿ� ���� ��� �����ϱ�
        else
        {
            canAttack = false;
            canMove = true;
            //��� ����
            
        }
    }
}
