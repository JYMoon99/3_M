using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{

    private void Update()
    {
        // ����Ʈ ��ġ ����
        Vector3 rightVec = transform.position;

        rightVec.y = 0.55f;

        transform.position = rightVec;  

        
    }
}
