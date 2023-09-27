using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type{ Ammo, Coin, Grenade, Heart, Weapon }
    public Type type;
    public int value;
    float moveSpeed = 2f;
    float delta = 0.2f;

    Vector3 currentY;

    private void Awake()
    {
    }

    private void Start()
    {
        currentY = transform.position;
    }

    private void Update()
    {
        ItemMoving();
    }





    #region 아이템 무빙
    void ItemMoving()
    {
        Vector3 itemVec = currentY;
        itemVec.y += delta * Mathf.Sin(Time.time * moveSpeed);
        transform.position = itemVec;

      
    }

    #endregion
}
