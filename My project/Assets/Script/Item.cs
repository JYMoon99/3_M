using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type{ Ammo, Coin, Grenade, Heart, Weapon }
    public Type type;
    public int value;
    protected float moveSpeed = 2f;
    protected float delta = 0.2f;

    protected Vector3 currentY;

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
        // Item 반복 운동
        Vector3 itemVec = currentY;
        itemVec.y += delta * Mathf.Sin(Time.time * moveSpeed);
        transform.position = itemVec;
    }

    #endregion
}
