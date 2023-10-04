using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        // 총알이 바닥과 충돌
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        // 총알이 벽과 충돌
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
