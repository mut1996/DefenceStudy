using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private TowerTemplate towerTemplate;
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

    public void SpawnTower(Transform tileTransfrom) 
    {



        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. Ÿ���� �Ǽ� �� ��ŭ ���� ������ Ÿ�� �Ǽ� x 
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ� �Ǿ� ������ Ÿ���Ǽ� x 
        if (tile.IsBuildTower == true) 
        {
    
            systemTextViewer.PrintText(SystemType.Build);
            return;

        }


        // Ÿ���� �Ǽ� �Ǿ� �������� ���� 
        tile.IsBuildTower = true;

        // Ÿ�� �Ǽ��� �ʿ��� ��� ��ŭ ����
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;

        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�(Ÿ�� ���� z�� -1�� ��ġ�� ��ġ)
        Vector3 position = tileTransfrom.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
       
       
        // Ÿ�� ���⿡ enemySpanwer���� ����
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner, playerGold);

    }
         

}


