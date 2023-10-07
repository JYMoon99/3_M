using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;


    // Awake()�� ��ӽÿ� �ڽ� ��ũ��Ʈ�� �ܵ� ����ȴ�.
    void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        enemyAni = GetComponentInChildren<Animator>();

        // ���� ����
        navAgent.isStopped = true;

        StartCoroutine(Think());
    }

    void Update()
    {
        // ���� ��� ó��
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        // ���� �ü� ó��
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5;
            transform.LookAt(target.position + lookVec);
        }
        else
            // ���� ���� ���� ���� ����
            navAgent.SetDestination(tauntVec);



    }

    // ���� ���� �ڷ�ƾ
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randomAction = Random.Range(0, 5);
        switch (randomAction)
        {
            case 0:
            case 1:
                // �̻��� �߻�
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                // ���� Rock
                StartCoroutine(RockShot());
                break;
            case 4:
                // ���� ����
                StartCoroutine(Taunt());
                break;
        }

    }

    // �̻��� ���� �ڷ�ƾ
    IEnumerator MissileShot()
    {
        enemyAni.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    // ���� Rock ���� �ڷ�ƾ
    IEnumerator RockShot()
    {
        isLook = false;
        enemyAni.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        isLook = false;
        navAgent.isStopped = false;
        boxCollider.enabled = false;
        enemyAni.SetTrigger("doTaunt");
        tauntVec = target.position + lookVec;

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        navAgent.isStopped = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}


