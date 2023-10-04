using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        // �Ѿ��� �ٴڰ� �浹
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        // �Ѿ��� ���� �浹
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
