using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public bool isInvincible;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;
    public ObjectSound objectSound = new ObjectSound();

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreTxt;
    public Text bestTxt;



    private void Awake()
    {
        // 각 스테이지에 등장할 몬스터 수 저장
        enemyList = new List<int>();

         if(!PlayerPrefs.HasKey("MaxScore"))
         {
            PlayerPrefs.SetInt("MaxScore", 0);
         }

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        Debug.Log("Menu MaxScore : " + maxScore);

        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        SoundManager.Instance.BgmSound(objectSound.bgmAudioClip[0]);
        


    }

    public void GameStart()
    {
        SoundManager.Instance.BgmStop();

        SoundManager.Instance.BgmSound(objectSound.bgmAudioClip[1]);

        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        SoundManager.Instance.BgmStop();
        SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[1]);


        curScoreTxt.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        Debug.Log("MaxScore : " + maxScore);
        Debug.Log("PlayerScore : " + player.score);
        
        if(player.score > maxScore)
        {
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
        else
            bestTxt.gameObject.SetActive(false);
        


        PlayerPrefs.Save();
        Debug.Log("MaxScore : " + maxScore);
    }

    public void ReStart()
    {
        SoundManager.Instance.BgmStop();

        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);


        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        isInvincible = false;
        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        SoundManager.Instance.BgmStop();
        SoundManager.Instance.BgmSound(objectSound.bgmAudioClip[1]);

        player.transform.position = Vector3.up * 0.65f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }

        isBattle = false;

        stage++;
    }

    IEnumerator InBattle()
    {
        if(stage % 5 == 0)
        {
            SoundManager.Instance.BgmStop();
            SoundManager.Instance.BgmSound(objectSound.bgmAudioClip[3]);

            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();
            enemy.manager = this;

        }
        else
        {
            SoundManager.Instance.BgmStop();
            SoundManager.Instance.BgmSound(objectSound.bgmAudioClip[2]);
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);

                // List 저장
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        // 반복문으로 Update()와 같은 역할
        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }
        isInvincible = true;
        SoundManager.Instance.BgmStop();
        SoundManager.Instance.SfxSound(objectSound.sfxAudioClip[0]);
        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();

    }

    private void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    private void LateUpdate() // LataUpdate() : Update()가 끝난 후 호출되는 생명주기
    {
        // 상단 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        playerHealthTxt.text = player.health + " / " + player.MaxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);

        // 플레이어 UI
       if (player.equipWeapon == null)
           playerAmmoTxt.text = " - / " + player.ammo;
       else if (player.equipWeapon.type == Weapon.Type.Melee)
           playerAmmoTxt.text = " - / " + player.ammo;
       else
           playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        // 무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        // 적 숫자 UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        // 보스 UI
        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector2.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1); // 보스의 체력이 둘 다 int형이라 하나만 float로 변경
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector2.up * 200;   
        }
        

    }

}
