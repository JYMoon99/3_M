using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody grenadeRigid;

    public int damage;

        private void Start()
    {
        StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        // 터질 때 물리적인 움직임 없애기
        grenadeRigid.velocity = Vector3.zero;
        grenadeRigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        // 파티클 ON
        effectObject.SetActive(true);

        // 수류탄에 맞은 적들 인식하기 위한 레이 구현
        RaycastHit[] rayHits = Physics.SphereCastAll(
            transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy")); // SphereCastAll : 구체 모양의 레이캐스팅

        foreach(RaycastHit hitObject in rayHits)
        {
            hitObject.transform.GetComponent<Enemy>().HitByGrenade(transform.position, gameObject);
        }

        Destroy(gameObject, 5f);

    }

    void Update()
    {
        
    }
}
