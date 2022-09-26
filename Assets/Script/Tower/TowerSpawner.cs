using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private TowerTemplate[] towerTemplate;      // Ÿ�� �� �� 
    //[SerializeField]
    //private GameObject towerPrefab;
    //[SerializeField]
    //private int towerBuildGold = 50;    // Ÿ�� �Ǽ��� ��ӿ�Ǵ� ���
    [SerializeField]
    private EnemySpawner enemySpawner; // ����ʿ� �����ϴ� �� ����Ʈ ������ �˱� ����..
    [SerializeField]
    private PlayerGold playerGold;      // Ÿ�� �Ǽ� �� ��� ���Ҹ� ����.. 
    [SerializeField]
    private SystemTextViewer systemTextViewer;  // �� ����, �Ǽ� �Ұ� ���� �ý��� �޽��� ��� 
    private bool isOnTowerButton = false;   // Ÿ�� �Ǽ� ��Ʈ�� �������� üũ
    private GameObject followTowerClone = null; // �ӽ� Ÿ���� �����ϴ� 
    private int towerType;


    public void ReadyToSpawnTower(int type) 
    {
        towerType = type;

        // ��ư�� �ߺ��ؼ� ������ ���� �����ϱ� ���� �ʿ� 
        if (isOnTowerButton == true) 
        {
            return;
        }

        // Ÿ�� �Ǽ� ���� �Ǵ�
        // Ÿ���� �Ǽ��� ��ŭ ���� ������ Ÿ���Ǽ� x 
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold) 
        {
            // ��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // Ÿ�� �Ǽ� ��ư�� �����ٰ� ���� 
        isOnTowerButton = true;
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);

        //Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransfrom) 
    {
        // Ÿ�� �Ǽ� ��ư�� ������ ���� Ÿ�� �Ǽ� ���� 
        if (isOnTowerButton == false) 
        {
            return;
        }

        /*
        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. Ÿ���� �Ǽ� �� ��ŭ ���� ������ Ÿ�� �Ǽ� x 
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
         */
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ� �Ǿ� ������ Ÿ���Ǽ� x 
        if (tile.IsBuildTower == true) 
        {
    
            systemTextViewer.PrintText(SystemType.Build);
            return;

        }

        // �ٽ� Ÿ�� �Ǽ� ���v�� ������ Ÿ���� �Ǽ��ϵ��� ���� ����
        isOnTowerButton = false;

        // Ÿ���� �Ǽ� �Ǿ� �������� ���� 
        tile.IsBuildTower = true;

        // Ÿ�� �Ǽ��� �ʿ��� ��� ��ŭ ����
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�(Ÿ�� ���� z�� -1�� ��ġ�� ��ġ)
        Vector3 position = tileTransfrom.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);

        // ���� ��ġ�ȴ� Ÿ���� ����Ÿ�� �ֺ��� ��ġ�� ���
        // ����ȿ���� ���� �� �ֶǷ� ��� ���� Ÿ���� ����ȿ�� ����
        OnBuffAllBuffTowers();

        // Ÿ�� ���⿡ enemySpanwer���� ����
        clone.GetComponent<TowerWeapon>().SetUp(this, enemySpawner, playerGold, tile);

        Destroy(followTowerClone);
        // Ÿ�� �Ǽ��� ������ ���֤��� �ڷ�ƾ �Լ� ���� 
        StopCoroutine("OnTowerCancelSystem");

    }


    private IEnumerator OnTowerCancelSystem() 
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) 
            {
                isOnTowerButton = false;
                // ���콺�� ����ٴϴ� �ӽ�Ÿ�� ����
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


