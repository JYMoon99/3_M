using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }
    public Type enemyType;

    public int maxHp;
    public int curHp;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    protected Rigidbody enemyRigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent navAgent;
    protected Animator enemyAni;

    private void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        enemyAni = GetComponentInChildren<Animator>();

        // 몬스터 대기 시간
        if(enemyType != Type.D)
            Invoke("EnemyStart", 2);    
    }

    private void Update()
    {
        EnemyMove();

    }

    void Targetting()
    {
        if (!isDead && enemyType != Type.D)
        {
            // 공격 범위 설정
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

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        enemyAni.SetBool("isAttack", true);

        switch (enemyType)
        {
            // 일단 몬스터 공격로직
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
        if(navAgent.enabled && enemyType != Type.D)
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


    private void OnTriggerExit(Collider other)
    {
        // 근접 공격과 원거리 공격 구별
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

            if(enemyType != Type.D)
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
            // 몬스터 피해 효과
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.red;
            }

            if (curHp > 0)
                yield return new WaitForSeconds(0.1f);

            if (curHp > 0)
            {
                foreach (MeshRenderer mesh in meshs)
                {
                    mesh.material.color = Color.white;
                }
            }
        }
        else
        {
            // 몬스터 Die로 레이어 변경
            gameObject.layer = 12;

            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            isChase = false;
            navAgent.enabled = false; // 사망시 넉백효과를 유지하기위해 NavAgent 비활성화
            enemyRigid.isKinematic = true;
            enemyAni.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);

            // Quaternion.identity 알아보기
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            switch(enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }



            if (isGrenade)
            {
                // 수류탄 피해 효과
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 10;

                enemyRigid.freezeRotation = false;
                enemyRigid.AddForce(reactVec * 5, ForceMode.Impulse);
                enemyRigid.AddTorque(reactVec * 10, ForceMode.Impulse);
            }
            else
            {
                // 기본 피해 효과
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 5;

                enemyRigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            // 죽은 몬스터 삭제
                Destroy(gameObject, 3f);
        }
    }
}

