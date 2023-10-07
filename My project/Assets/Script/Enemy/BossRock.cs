using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    // 회전력
    float angularPower = 2;
    // 크기
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
            // 크기와 회전력이 점차 커지는 로직
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); // Acceleration : 가속하여 속도를 가함
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
