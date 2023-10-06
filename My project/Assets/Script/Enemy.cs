using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C }
    public Type enemyType;

    public int maxHp;
    public int curHp;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;

    Rigidbody enemyRigid;
    BoxCollider boxCollider;
    MeshRenderer[] mats;
    NavMeshAgent navAgent;
    Animator enemyAni;

    private void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>(); // material은 MeshRenderer가 정보를 가지고있다.
        navAgent = GetComponent<NavMeshAgent>();
        enemyAni = GetComponentInChildren<Animator>();

        // 몬스터 대기 시간
        Invoke("EnemyStart", 2);
    }

    private void Update()
    {
        EnemyMove();

    }

    void Targetting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enemyType)
        {
            case Type.A:
                 targetRadius = 1.5f;
                 targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 10f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;

        }
            

        RaycastHit[] rayHits = Physics.SphereCastAll(
            transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        enemyAni.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                enemyRigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                enemyRigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                if (curHp > 0)
                {
                    GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                    Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    rigidBullet.velocity = transform.forward * 20;
                }

                yield return new WaitForSeconds(2f);
                break;
        }

       
        isChase = true;
        isAttack = false;
        enemyAni.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }

    private void EnemyMove()
    {
        if(navAgent.enabled)
        {
            navAgent.SetDestination(target.position); // SetDestination() : 도착할 목표 위치 지정 함수
            navAgent.isStopped = !isChase;

        }
        
    }

    void EnemyStart()
    {
        isChase = true;
        enemyAni.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            enemyRigid.velocity = Vector3.zero;
            enemyRigid.angularVelocity = Vector3.zero; // angularVelocity : 물리 회전 속도
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            //Debug.Log("curHp : " + curHp);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.damage;
            // Debug.Log("curHp : " + curHp);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StopCoroutine(OnDamage(reactVec, false));
            StartCoroutine(OnDamage(reactVec, false));
        }

    }

    public void HitByGrenade(Vector3 explosionPos, GameObject gameObject)
    {
        Grenade grenade = gameObject.GetComponent<Grenade>();
        curHp -= grenade.damage;
        // 적과 수류탄의 거리 계산
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec,true));
    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {

        if (curHp > 0)
        {
            foreach (MeshRenderer mesh in mats)
            {
                mesh.material.color = Color.red;
            }

            yield return new WaitForSeconds(0.1f);
            if (curHp > 0)
            {
                foreach (MeshRenderer mesh in mats)
                {
                    mesh.material.color = Color.white;
                }
            }
        }
        else
        {
            gameObject.layer = 12;

            foreach (MeshRenderer mesh in mats)
            {
                mesh.material.color = Color.gray;
            }
            isChase = false;
            navAgent.enabled = false; // 사망시 넉백효과를 유지하기위해 NavAgent 비활성화
            enemyAni.SetTrigger("doDie");


            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                enemyRigid.freezeRotation = false;
                enemyRigid.AddForce(reactVec * 5, ForceMode.Impulse);
                enemyRigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                enemyRigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }


            
                Destroy(gameObject, 3f);
        }
    }
}

