using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{

    private void Update()
    {
        // Vector3 itemVec = currentY;
        // itemVec.y += delta * Mathf.Sin(Time.time * moveSpeed);

        Vector3 itemVec = transform.position;

        itemVec.y = 0.55f;

        transform.position = itemVec;  

        
    }
}
