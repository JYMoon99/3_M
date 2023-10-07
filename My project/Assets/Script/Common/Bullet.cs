using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;



    private void OnCollisionEnter(Collision collision)
    {
        // �Ѿ��� �ٴڰ� �浹
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �Ѿ��� ���� �浹
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
