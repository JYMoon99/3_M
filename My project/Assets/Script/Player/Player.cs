using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] ObjectSound objectSound = new ObjectSound();

    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public GameObject grenadeObject;
    public Camera followCamera;
    public GameManager manager;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    public int score;

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
    bool xDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isShopping;
    bool isDead;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    public Weapon equipWeapon;
    MeshRenderer[] playerMeshs;

    public int equipWeaponIndex = -1;
    float fireDelay;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        playerMeshs = GetComponentsInChildren<MeshRenderer>();

       // PlayerPrefs.SetInt("MaxScore", 0); // PlayerPrefs : ����Ƽ���� �����ϴ� ������ ���� ���
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
        Dodge();
        Grenade();
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
        xDown = Input.GetButtonDown("NullWeapon");
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

        // Ư�� ��Ȳ���� �������� �ʰ� ����
        if(isSwap || !isFireReady || isReload || isDead) { moveVec = Vector3.zero; }

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

        if (fDown && !isDead)
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
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isReload && !isDead) 
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

        if(gDown && !isDead) 
        {
            SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[3]);

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
    {   // ������ �Ȱ��� jDown(space)��� ������ �÷��̾ �����̰� ���� ���� ������ ���Ⱑ ���� ���
        if (jDown && !isJump && !isDodge && equipWeapon != null && !isDead)
        {
            SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[2]);

            if (isReload)
            {
                isReload = false;
                anim.SetTrigger("doDodge");
                speed *= 2;
                Invoke("DodgeOut", 0.4f); // �ð��� �Լ� ȣ��
                return;
            }

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

        if (isReload)
            return;

        fireDelay += Time.deltaTime; // Time.deltaTime : 1������ �� �ɸ��� �ð�
        // ��� �ð����� ���� ������ ���ݼӵ��� ������ �����غ� true
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isShopping && !isDead)
        {

            if (equipWeaponIndex == 1)
            {
                SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[0]);
            }
            else if (equipWeaponIndex == 2)
            {
                SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[1]);
            }
            else if (equipWeaponIndex == 0)
            {
                SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[4]);
            }

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

        if (ammo == 0 || equipWeapon.curAmmo == equipWeapon.maxAmmo) return;



        if (rDown && !isJump && !isDodge && !isSwap && !isReload && isFireReady && !isShopping && !isDead) 
        {
            // �����ϸ鼭 ���� ��� ���� ����

            anim.SetTrigger("doReload");
            isReload = true;

            StartCoroutine(ReloadOut());
        }
    }

    IEnumerator ReloadOut()
    {
        // ������ �� �Ѿ��� ���� ����
        yield return new WaitForSeconds(2f);

        if(isDodge)
        {
            Debug.Log("isDodge");
        }

        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo - equipWeapon.curAmmo;

            equipWeapon.curAmmo += reAmmo;
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
            else if(nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShopping = true;
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
        if (other.tag == "Item")
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
                    if (hasGrenades >= MaxHasGrenades)
                        return;

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
            // �Ϲ� ����
            Bullet enemyBullet = other.GetComponent<Bullet>();

            if(!manager.isInvincible)
            health -= enemyBullet.damage;

            // ���� ����
            bool isBossAtk = other.name == "Boss Melee Area";
            StartCoroutine(OnDamage(isBossAtk));

            // �� ���� ������Ʈ�� �浹�ϸ� �ش� ������Ʈ ����
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);

        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[5]);

        foreach(MeshRenderer mesh in playerMeshs)
        {
            mesh.material.color = Color.red;
        }

        if(isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);


        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        foreach (MeshRenderer mesh in playerMeshs)
        {
            mesh.material.color = Color.white;
        }

        if(isBossAtk)
            rigid.velocity = Vector3.zero;
    }

    private void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;

        manager.GameOver();

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
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
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShopping = false;
            nearObject = null;
        }
    }
}
