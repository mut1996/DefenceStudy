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
    //private int towerBuildGold = 50;    // 타워 건설에 사ㅣ용되는 골드
    [SerializeField]
    private EnemySpawner enemySpawner; // 현재맵에 존재하는 적 리스트 정보를 알기 위해..
    [SerializeField]
    private PlayerGold playerGold;      // 타워 건설 시 골드 감소를 위해.. 
    [SerializeField]
    private SystemTextViewer systemTextViewer;  // 돈 부족, 건설 불가 같은 시스템 메시지 출력 

    public void SpawnTower(Transform tileTransfrom) 
    {



        // 타워 건설 가능 여부 확인
        // 1. 타워를 건설 할 만큼 돈이 없으면 타워 건설 x 
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. 현재 타일의 위치에 이미 타워가 건설 되어 있으면 타워건설 x 
        if (tile.IsBuildTower == true) 
        {
    
            systemTextViewer.PrintText(SystemType.Build);
            return;

        }


        // 타워가 건설 되어 있음으로 설정 
        tile.IsBuildTower = true;

        // 타워 건설에 필요한 골드 만큼 감소
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;

        // 선택한 타일의 위치에 타워 건설(타일 보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransfrom.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
       
       
        // 타위 무기에 enemySpanwer정보 전달
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner, playerGold);

    }
         

}


