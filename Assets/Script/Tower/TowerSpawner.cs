using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private TowerTemplate[] towerTemplate;      // 타워 정 보 
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
    private bool isOnTowerButton = false;   // 타워 건설 버트을 눌렀는지 체크
    private GameObject followTowerClone = null; // 임시 타워를 저장하는 
    private int towerType;


    public void ReadyToSpawnTower(int type) 
    {
        towerType = type;

        // 버튼을 중복해서 누르는 것을 방지하기 위해 필요 
        if (isOnTowerButton == true) 
        {
            return;
        }

        // 타워 건설 여부 판단
        // 타워를 건설할 만큼 돈이 없으면 타워건설 x 
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold) 
        {
            // 골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // 타워 건설 버튼을 눌렀다고 설정 
        isOnTowerButton = true;
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);

        //타워 건설을 취소할 수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransfrom) 
    {
        // 타워 건설 버튼을 눌렀을 떄만 타워 건설 가능 
        if (isOnTowerButton == false) 
        {
            return;
        }

        /*
        // 타워 건설 가능 여부 확인
        // 1. 타워를 건설 할 만큼 돈이 없으면 타워 건설 x 
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
         */
        
        
        
        Tile tile = tileTransfrom.GetComponent<Tile>();
        
        
        // 2. 현재 타일의 위치에 이미 타워가 건설 되어 있으면 타워건설 x 
        if (tile.IsBuildTower == true) 
        {
    
            systemTextViewer.PrintText(SystemType.Build);
            return;

        }

        // 다시 타워 건설 버틍르 눌러서 타워를 건설하도록 변수 설정
        isOnTowerButton = false;

        // 타워가 건설 되어 있음으로 설정 
        tile.IsBuildTower = true;

        // 타워 건설에 필요한 골드 만큼 감소
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        // 선택한 타일의 위치에 타워 건설(타일 보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransfrom.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);

        // 새로 배치된는 타워가 버프타워 주변에 배치될 경우
        // 버프효과를 받을 수 있또록 모든 버프 타워의 버프효과 갱신
        OnBuffAllBuffTowers();

        // 타위 무기에 enemySpanwer정보 전달
        clone.GetComponent<TowerWeapon>().SetUp(this, enemySpawner, playerGold, tile);

        Destroy(followTowerClone);
        // 타워 건설을 츃소할 수있ㄴ는 코루틴 함수 중지 
        StopCoroutine("OnTowerCancelSystem");

    }


    private IEnumerator OnTowerCancelSystem() 
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) 
            {
                isOnTowerButton = false;
                // 마우스를 따라다니는 임시타워 삭제
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


