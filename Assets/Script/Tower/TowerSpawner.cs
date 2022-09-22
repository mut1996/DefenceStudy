using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50;    // 타워 건설에 사ㅣ용되는 골드
    [SerializeField]
    private EnemySpawner enemySpawner; // 현재맵에 존재하는 적 리스트 정보를 알기 위해..
    [SerializeField]
    private PlayerGold playerGold;      // 타워 건설 시 골드 감소를 위해.. 

    public void SpawnTower(Transform tileTransfrom) 
    {



        // 타워 건설 가능 여부 확인
        // 1. 타워를 건설 할 만큼 돈이 없으면 타워 건설 x 
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. 현재 타일의 위치에 이미 타워가 건설 되어 있으면 타워건설 x 
        if (tile.IsBuildTower == true) 
        {
            return;
        }


        // 타워가 건설 되어 있음으로 설정 
        tile.IsBuildTower = true;

        // 타워 건설에 필요한 골드 만큼 감소
        playerGold.CurrentGold -= towerBuildGold;

        // 선택한 타일의 위치에 타워 건설 
        GameObject clone = Instantiate(towerPrefab, tileTransfrom.position, Quaternion.identity);
       
        // 타위 무기에 enemySpanwer정보 전달
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner);

    }
         

}


