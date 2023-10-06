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
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
