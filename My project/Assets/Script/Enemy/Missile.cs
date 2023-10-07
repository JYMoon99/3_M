using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Missile : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        // 미사일 회전 효과
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
