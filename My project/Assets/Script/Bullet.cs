using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isMelee;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        // 총알이 바닥과 충돌
        if (other.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }

        // 총알이 벽과 충돌
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
