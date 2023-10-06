using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public GameObject grenadeObject;
    public Camera followCamera;
    
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHealth;
    public int MaxHasGrenades;


    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool gDown;
    bool rDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    Weapon equipWeapon;
    MeshRenderer[] playerMeshs;

    int equipWeaponIndex = -1;
    float fireDelay;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        playerMeshs = GetComponentsInChildren<MeshRenderer>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Dodge();
        Attack();
        Reload();
        Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        
    }

    void Move()
    {
        // ������ ����
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge) 
        {
            // ȸ���߿��� ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�
            moveVec = dodgeVec;
        }

        // ���� �������϶��� �������� �ʰ� ����
        if(isSwap) { moveVec = Vector3.zero; }

        // �ȱ� �ӵ��� 0.3f / ���� �ӵ� 1f
        if(!isBorder)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        // ���Ͱ� 0�� �ƴϸ� �ִϸ��̼� "isRun" ����
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); // LookAt() : ������ ���͸� ���ؼ� ȸ�������ִ� �Լ�

        // ���콺�� ���� ȸ��
        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() : ��ũ������ ����� Ray�� ��� �Լ�
        RaycastHit rayHit;

        if (fDown)
        {
            // Physics.Raycast�� ������ ���� �����Ͽ� ��� ���ߵǸ� true�� �����ϴ� ���� �Լ���.
            if (Physics.Raycast(ray, out rayHit, 100)) // out : returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
            {
                // �浹�ϴ� ���� �Ÿ� ���
                Vector3 nextVec = rayHit.point - transform.position;
                // ĳ���Ͱ� ���� �Ĵٺ��� �ʰ� �ϱ�
                nextVec.y = 0;
                // ĳ���Ͱ� ���콺 ����Ʈ�� �Ĵٺ��� ����
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump()
    {
        // �÷��̾ �������� ������ ���� ����
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge) 
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // ForceMode.Impulse : ��������� ���� ����
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            // isJump = true�� �����ν� �������� ����
            isJump = true;
        }
    }


    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown) 
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() : ��ũ������ ����� Ray�� ��� �Լ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
            {
                // �浹�ϴ� ���� �Ÿ� ���
                Vector3 nextVec = rayHit.point - transform.position;
                // ����ź ���� ������
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
        
            }
        }

        
    }


    void Dodge()
    {   // ������ �Ȱ��� jDown(space)��� ������ �÷��̾ �����̰� ���� �� true
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            // ���� �Ҷ��� ���Ͱ��� �������Ϳ� ����
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // ���� 0.4f�� �� ���ǵ尪 ����ȭ
            Invoke("DodgeOut", 0.4f); // �ð��� �Լ� ȣ��
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Attack()
    {
        // ������ ���Ⱑ ������ ���� ����
        if (equipWeapon == null)
            return;

        // ��� �ð� ����
        fireDelay += Time.deltaTime; // Time.deltaTime : 1������ �� �ɸ��� �ð�
        // ��� �ð����� ���� ������ ���ݼӵ��� ������ �����غ� true
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            // ���� �����϶��� ���� : �������� ��
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            // ��� �ð� �ʱ�ȭ
            fireDelay = 0;

        }
    }

    void Reload()
    {
        // ���� �̸� ����
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (ammo == 0) return;

        if (rDown && !isJump && !isDodge && !isSwap && !isReload && isFireReady) 
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1f);
        }
    }

    void ReloadOut()
    {
        // ���ľ� �� �� : �Ѿ��� curAmmo�� ������� Ammo���� equipWeapon.maxAmmo�� �Ѿ��� �����´�.
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    void Swap()
    {
        // ���⸦ �������϶� ���� ���⸦ �ٽ� ���� ���� && �ش� ���⸦ �����ϰ� ���� ���� �� ���� ����
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if (sDown1 || sDown2 || sDown3)
        {
            // �������� ���Ⱑ �������� �ش� ���⸦ ��Ȱ��ȭ
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);

            // ���ҵ��߿��� �̵��Ұ�
            moveVec = Vector3.zero;
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if(iDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                // �ش� ���� �ε����� ����
                hasWeapons[weaponIndex] = true;
                // ����� ���� ����
                Destroy(nearObject);
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; // angularVelocity : ���� ȸ�� �ӵ�
    }

    void StopToWall()
    {
        // ĳ���Ͱ� �����̴� ������ Scene�� ���̴� Ray�� �׸� 
        Debug.DrawRay(transform.position, moveVec * 5, Color.green); // DrawRay() : Scene������ Ray�� �����ִ� �Լ�
        // ĳ���Ͱ� �����̴� �������� Ray�� ���� ���� �ν�
        isBorder = Physics.Raycast(transform.position, moveVec, 5, LayerMask.GetMask("Wall"));
    }
   
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }


    private void OnTriggerEnter(Collider other)
    {
        // ������ ȹ�� �� Max �� ����
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo >= MaxAmmo)
                        ammo = MaxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin >= MaxCoin)
                        coin = MaxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health >= MaxHealth) 
                        health = MaxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades >= MaxHasGrenades)
                        hasGrenades = MaxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);

        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                if (other.GetComponent<Rigidbody>() != null)
                    Destroy(other.gameObject);

                StartCoroutine(OnDamage());
            }

        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach(MeshRenderer mesh in playerMeshs)
        {
            mesh.material.color = Color.blue;
        }
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in playerMeshs)
        {
            mesh.material.color = Color.white;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
