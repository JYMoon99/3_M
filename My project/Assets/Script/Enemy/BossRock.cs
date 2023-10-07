using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    // ȸ����
    float angularPower = 2;
    // ũ��
    float scaleValue = 0.1f;
    bool isShoot;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;

    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            // ũ��� ȸ������ ���� Ŀ���� ����
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); // Acceleration : �����Ͽ� �ӵ��� ����
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
