using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type{ Ammo, Coin, Grenade, Heart, Weapon }
    public Type type;
    public int value;

    GameObject gameobject;

    float itemRotate = 0.1f;


    private void Awake()
    {
    }

    private void Update()
    {
        transform.Rotate(0, itemRotate, 0);
    }


}
