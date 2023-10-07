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
        // ���� �� �������� ������ ���ֱ�
        grenadeRigid.velocity = Vector3.zero;
        grenadeRigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        // ��ƼŬ ON
        effectObject.SetActive(true);

        // ����ź�� ���� ���� �ν��ϱ� ���� ���� ����
        RaycastHit[] rayHits = Physics.SphereCastAll(
            transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy")); // SphereCastAll : ��ü ����� ����ĳ����

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
