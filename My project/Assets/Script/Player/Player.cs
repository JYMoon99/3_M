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

       // PlayerPrefs.SetInt("MaxScore", 0); // PlayerPrefs : 유니티에서 제공하는 간단한 저장 기능
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
        // 움직임 구현
        
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge) 
        {
            // 회피중에는 움직임 벡터 -> 회피방향 벡터로 바꿈
            moveVec = dodgeVec;
        }

        // 특정 상황에서 움직이지 않게 구현
        if(isSwap || !isFireReady || isReload || isDead) { moveVec = Vector3.zero; }

        // 걷기 속도는 0.3f / 평상시 속도 1f
        if(!isBorder)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        // 벡터가 0이 아니면 애니메이션 "isRun" 동작
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // LookAt() : 지정된 벡터를 향해서 회전시켜주는 함수

        // 마우스에 의한 회전
        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() : 스크린에서 월드로 Ray를 쏘는 함수
        RaycastHit rayHit;

        if (fDown && !isDead)
        {
            // Physics.Raycast는 직선을 씬에 투영하여 대상에 적중되면 true를 리턴하는 물리 함수다.
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                // 충돌하는 지점 거리 계산
                Vector3 nextVec = rayHit.point - transform.position;
                // 캐릭터가 위를 쳐다보지 않게 하기
                nextVec.y = 0;
                // 캐릭터가 마우스 포인트를 쳐다보게 구현
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump()
    {
        // 플레이어가 움직이지 않을때 점프 가능
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isReload && !isDead) 
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // ForceMode.Impulse : 즉발적으로 힘을 가함
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            // isJump = true를 함으로써 더블점프 막기
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

            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() : 스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                // 충돌하는 지점 거리 계산
                Vector3 nextVec = rayHit.point - transform.position;
                // 수류탄 높이 던지기
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
    {   // 점프와 똑같은 jDown(space)사용 조건은 플레이어가 움직이고 있을 때와 장착된 무기가 있을 경우
        if (jDown && !isJump && !isDodge && equipWeapon != null && !isDead)
        {
            SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[2]);

            if (isReload)
            {
                isReload = false;
                anim.SetTrigger("doDodge");
                speed *= 2;
                Invoke("DodgeOut", 0.4f); // 시간차 함수 호출
                return;
            }

                // 닷지 할때의 벡터값을 닷지벡터에 저장
                dodgeVec = moveVec;
                speed *= 2;
                anim.SetTrigger("doDodge");
                isDodge = true;

                // 닷지 0.4f초 후 스피드값 정상화
                Invoke("DodgeOut", 0.4f); // 시간차 함수 호출
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Attack()
    {
        // 장착된 무기가 없을때 공격 방지
        if (equipWeapon == null)
            return;

        if (isReload)
            return;

        fireDelay += Time.deltaTime; // Time.deltaTime : 1프레임 당 걸리는 시간
        // 대기 시간보다 장착 무기의 공격속도가 낮으면 공격준비 true
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
            // 근접 무기일때는 스윙 : 나머지는 샷
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            // 대기 시간 초기화
            fireDelay = 0;



        }
    }

    void Reload()
    {
        // 오류 미리 방지
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (ammo == 0 || equipWeapon.curAmmo == equipWeapon.maxAmmo) return;



        if (rDown && !isJump && !isDodge && !isSwap && !isReload && isFireReady && !isShopping && !isDead) 
        {
            // 장전하면서 총을 쏘는 버그 방지

            anim.SetTrigger("doReload");
            isReload = true;

            StartCoroutine(ReloadOut());
        }
    }

    IEnumerator ReloadOut()
    {
        // 장전할 때 총알의 개수 로직
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
        // 무기를 장착중일때 같은 무기를 다시 장착 방지 && 해당 무기를 소유하고 있지 않을 때 장착 방지
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
            // 장착중인 무기가 있을때만 해당 무기를 비활성화
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

            // 스왑도중에는 이동불가
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
                // 해당 무기 인덱스값 저장
                hasWeapons[weaponIndex] = true;
                // 드롭한 무기 제거
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
        rigid.angularVelocity = Vector3.zero; // angularVelocity : 물리 회전 속도
    }

    void StopToWall()
    {
        // 캐릭터가 움직이는 방향대로 Scene에 보이는 Ray를 그림 
        Debug.DrawRay(transform.position, moveVec * 5, Color.green); // DrawRay() : Scene내에서 Ray를 보여주는 함수
        // 캐릭터가 움직이는 방향으로 Ray를 쏴서 벽을 인식
        isBorder = Physics.Raycast(transform.position, moveVec, 5, LayerMask.GetMask("Wall"));
    }
   
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }


    private void OnTriggerEnter(Collider other)
    {
        // 아이템 획득 및 Max 값 설정
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
            // 일반 어택
            Bullet enemyBullet = other.GetComponent<Bullet>();

            if(!manager.isInvincible)
            health -= enemyBullet.damage;

            // 보스 어택
            bool isBossAtk = other.name == "Boss Melee Area";
            StartCoroutine(OnDamage(isBossAtk));

            // 적 공격 오브젝트가 충돌하면 해당 오브젝트 삭제
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
