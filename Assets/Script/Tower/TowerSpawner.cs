using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private TowerTemplate[] towerTemplate;      // Å¸¿ö Á¤ º¸ 
    //[SerializeField]
    //private GameObject towerPrefab;
    //[SerializeField]
    //private int towerBuildGold = 50;    // Å¸¿ö °Ç¼³¿¡ »ç¤Ó¿ëµÇ´Â °ñµå
    [SerializeField]
    private EnemySpawner enemySpawner; // ÇöÀç¸Ê¿¡ Á¸ÀçÇÏ´Â Àû ¸®½ºÆ® Á¤º¸¸¦ ¾Ë±â À§ÇØ..
    [SerializeField]
    private PlayerGold playerGold;      // Å¸¿ö °Ç¼³ ½Ã °ñµå °¨¼Ò¸¦ À§ÇØ.. 
    [SerializeField]
    private SystemTextViewer systemTextViewer;  // µ· ºÎÁ·, °Ç¼³ ºÒ°¡ °°Àº ½Ã½ºÅÛ ¸Ş½ÃÁö Ãâ·Â 
    private bool isOnTowerButton = false;   // Å¸¿ö °Ç¼³ ¹öÆ®À» ´­·¶´ÂÁö Ã¼Å©
    private GameObject followTowerClone = null; // ÀÓ½Ã Å¸¿ö¸¦ ÀúÀåÇÏ´Â 
    private int towerType;


    public void ReadyToSpawnTower(int type) 
    {
        towerType = type;

        // ¹öÆ°À» Áßº¹ÇØ¼­ ´©¸£´Â °ÍÀ» ¹æÁöÇÏ±â À§ÇØ ÇÊ¿ä 
        if (isOnTowerButton == true) 
        {
            return;
        }

        // Å¸¿ö °Ç¼³ ¿©ºÎ ÆÇ´Ü
        // Å¸¿ö¸¦ °Ç¼³ÇÒ ¸¸Å­ µ·ÀÌ ¾øÀ¸¸é Å¸¿ö°Ç¼³ x 
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold) 
        {
            // °ñµå°¡ ºÎÁ·ÇØ¼­ Å¸¿ö °Ç¼³ÀÌ ºÒ°¡´ÉÇÏ´Ù°í Ãâ·Â
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // Å¸¿ö °Ç¼³ ¹öÆ°À» ´­·¶´Ù°í ¼³Á¤ 
        isOnTowerButton = true;
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);

        //Å¸¿ö °Ç¼³À» Ãë¼ÒÇÒ ¼ö ÀÖ´Â ÄÚ·çÆ¾ ÇÔ¼ö ½ÃÀÛ
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransfrom) 
    {
        // Å¸¿ö °Ç¼³ ¹öÆ°À» ´­·¶À» ‹š¸¸ Å¸¿ö °Ç¼³ °¡´É 
        if (isOnTowerButton == false) 
        {
            return;
        }

        /*
        // Å¸¿ö °Ç¼³ °¡´É ¿©ºÎ È®ÀÎ
        // 1. Å¸¿ö¸¦ °Ç¼³ ÇÒ ¸¸Å­ µ·ÀÌ ¾øÀ¸¸é Å¸¿ö °Ç¼³ x 
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //°ñµå°¡ ºÎÁ·ÇØ¼­ Å¸¿ö °Ç¼³ÀÌ ºÒ°¡´ÉÇÏ´Ù°í Ãâ·Â
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
         */
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. ÇöÀç Å¸ÀÏÀÇ À§Ä¡¿¡ ÀÌ¹Ì Å¸¿ö°¡ °Ç¼³ µÇ¾î ÀÖÀ¸¸é Å¸¿ö°Ç¼³ x 
        if (tile.IsBuildTower == true) 
        {
    
            systemTextViewer.PrintText(SystemType.Build);
            return;

        }

        // ´Ù½Ã Å¸¿ö °Ç¼³ ¹öºv¸£ ´­·¯¼­ Å¸¿ö¸¦ °Ç¼³ÇÏµµ·Ï º¯¼ö ¼³Á¤
        isOnTowerButton = false;

        // Å¸¿ö°¡ °Ç¼³ µÇ¾î ÀÖÀ½À¸·Î ¼³Á¤ 
        tile.IsBuildTower = true;

        // Å¸¿ö °Ç¼³¿¡ ÇÊ¿äÇÑ °ñµå ¸¸Å­ °¨¼Ò
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        // ¼±ÅÃÇÑ Å¸ÀÏÀÇ À§Ä¡¿¡ Å¸¿ö °Ç¼³(Å¸ÀÏ º¸´Ù zÃà -1ÀÇ À§Ä¡¿¡ ¹èÄ¡)
        Vector3 position = tileTransfrom.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);

        // »õ·Î ¹èÄ¡µÈ´Â Å¸¿ö°¡ ¹öÇÁÅ¸¿ö ÁÖº¯¿¡ ¹èÄ¡µÉ °æ¿ì
        // ¹öÇÁÈ¿°ú¸¦ ¹ŞÀ» ¼ö ÀÖ¶Ç·Ï ¸ğµç ¹öÇÁ Å¸¿öÀÇ ¹öÇÁÈ¿°ú °»½Å
        OnBuffAllBuffTowers();

        // Å¸À§ ¹«±â¿¡ enemySpanwerÁ¤º¸ Àü´Ş
        clone.GetComponent<TowerWeapon>().SetUp(this, enemySpawner, playerGold, tile);

        Destroy(followTowerClone);
        // Å¸¿ö °Ç¼³À» ®¼ÒÇÒ ¼öÀÖ¤¤´Â ÄÚ·çÆ¾ ÇÔ¼ö ÁßÁö 
        StopCoroutine("OnTowerCancelSystem");

    }


    private IEnumerator OnTowerCancelSystem() 
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) 
            {
                isOnTowerButton = false;
                // ¸¶¿ì½º¸¦ µû¶ó´Ù´Ï´Â ÀÓ½ÃÅ¸¿ö »èÁ¦
                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }

    }

    public void OnBuffAllBuffTowers() 
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i) 
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if (weapon.WeaponType == WeaponType.Buff) 
            {
                weapon.OnBuffAroundTower();
            }
        }
    }

}


