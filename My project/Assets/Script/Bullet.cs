using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        // �Ѿ��� �ٴڰ� �浹
        if (other.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }

        // �Ѿ��� ���� �浹
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
