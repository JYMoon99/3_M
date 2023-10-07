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


    // Awake()는 상속시에 자식 스크립트만 단독 실행된다.
    void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        enemyAni = GetComponentInChildren<Animator>();

        // 보스 멈춤
        navAgent.isStopped = true;

        StartCoroutine(Think());
    }

    void Update()
    {
        // 보스 사망 처리
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        // 보스 시선 처리
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5;
            transform.LookAt(target.position + lookVec);
        }
        else
            // 보스 점프 공격 패턴 실행
            navAgent.SetDestination(tauntVec);



    }

    // 보스 패턴 코루틴
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randomAction = Random.Range(0, 5);
        switch (randomAction)
        {
            case 0:
            case 1:
                // 미사일 발사
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                // 보스 Rock
                StartCoroutine(RockShot());
                break;
            case 4:
                // 점프 공격
                StartCoroutine(Taunt());
                break;
        }

    }

    // 미사일 패턴 코루틴
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

    // 보스 Rock 패턴 코루틴
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


