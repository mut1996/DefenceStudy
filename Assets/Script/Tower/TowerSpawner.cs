using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50;    // Ÿ�� �Ǽ��� ��ӿ�Ǵ� ���
    [SerializeField]
    private EnemySpawner enemySpawner; // ����ʿ� �����ϴ� �� ����Ʈ ������ �˱� ����..
    [SerializeField]
    private PlayerGold playerGold;      // Ÿ�� �Ǽ� �� ��� ���Ҹ� ����.. 

    public void SpawnTower(Transform tileTransfrom) 
    {



        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. Ÿ���� �Ǽ� �� ��ŭ ���� ������ Ÿ�� �Ǽ� x 
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ� �Ǿ� ������ Ÿ���Ǽ� x 
        if (tile.IsBuildTower == true) 
        {
            return;
        }


        // Ÿ���� �Ǽ� �Ǿ� �������� ���� 
        tile.IsBuildTower = true;

        // Ÿ�� �Ǽ��� �ʿ��� ��� ��ŭ ����
        playerGold.CurrentGold -= towerBuildGold;

        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ� 
        GameObject clone = Instantiate(towerPrefab, tileTransfrom.position, Quaternion.identity);
       
        // Ÿ�� ���⿡ enemySpanwer���� ����
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner);

    }
         

}


