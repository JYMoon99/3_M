using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    public int curHp;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;



    public float speed;




    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material; // material은 MeshRenderer가 정보를 가지고있다.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Debug.Log("curHp : " + curHp);
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
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (curHp > 0)
            {
                mat.color = Color.white;
            }
        }
        else
        {
            gameObject.layer = 12;
            mat.color = Color.gray;

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

                Destroy(gameObject, 3f);
        }
    }
}

