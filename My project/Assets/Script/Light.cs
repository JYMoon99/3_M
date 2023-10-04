using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{

    private void Update()
    {
        // 라이트 위치 고정
        Vector3 rightVec = transform.position;

        rightVec.y = 0.55f;

        transform.position = rightVec;  

        
    }
}
