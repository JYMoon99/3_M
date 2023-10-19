using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer traileffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;


    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        if(type == Type.Range && curAmmo > 0) {
            curAmmo--;
            StartCoroutine("Shot");

        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        traileffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        traileffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // ÃÑ¾Ë »ý¼º
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // Instantiate() : ¿ÀºêÁ§Æ® »ý¼º
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;
        // ÅºÇÇ »ý¼º
        GameObject instantBulletCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-1, -3) + Vector3.up * Random.Range(1, 3);
        // ÅºÇÇ Æ¨°Ü³ª°¨
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        // ÅºÇÇ È¸Àü ±¸Çö
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
