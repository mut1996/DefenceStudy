using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab;      // 적 프리팹
    [SerializeField]
    private GameObject enemyHPSliderPrefab; //적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;  // UI 를 표현하는 Canvas 오브젝트의 Trasform
    //[SerializeField]
    //private float spawnTime;             // 스폰 시간 
    [SerializeField]
    private Transform[] wayPoints;      // 현재 스테이지의 이동 경로
    [SerializeField]
    private PlayerHP playerHP;          // 플레이어 체력 컴포넌트 
    [SerializeField]
    private PlayerGold playerGold;      // 플레이어 골드 컴포넌트 
    private Wave currentWave;           // 현재 웨이브 정보 
    private int currentEnemyCount;      // 현재 웨이브에 남아있는 적 숫자( 웨이브 시작시 max 로설정, 적 사망시 -1)
    private List<Enemy> enemyList;      // 현재 맵에 존재하는 모든 적의 정보


    // 적의 생성과 삭제는 EnemySpawner에서 하기 때문에 set은 필요없다
    public List<Enemy> EnemyList => enemyList;

    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;


    private void Awake()
    {
        enemyList = new List<Enemy>();

        // 적 생성 코루틴 함수 호출
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave) 
    {
        // 매개 변수로 받아온 웨이브 정보저장
        currentWave = wave;
        // 현재 웨이브의 최대 적 숫자를 저장
        currentEnemyCount = currentWave.maxEnemyCount;
        // 현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy() 
    {
        // 현재웨이브에 생성중인 적    
        int spawnEnemyCount = 0;


        //while (true)
        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // GameObject clone = Instantiate(enemyPrefab);
            // 웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하도록 설정하고, 적 오브젝트 생성
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);               // 리스트에 방금 생성된 적 정보를 저장 

            SpawnEnemyHPSlider(clone);          // 적 체력을 나타는s sliderUI 생성 및 설정 

            // 현재 웨이브에서 생성한 적의 숫자 + 1 
            spawnEnemyCount++;

            // 각 웨ㅂ이브 마다 spawnTime 이 다를 수 있기 때문에 현재 웨이브(currentWave) 의 spawnTIme 사용 
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }



    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold) 
    {

        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }

        // 적이 플레이어의 발사체에게 사망 했을 때
        else if (type == EnemyDestroyType.Kill)
        {
            // 적의 종류에 따라 사망시 골드 획득 
            playerGold.CurrentGold += gold;
        }
        // 적이 사망할 떄마다 현재 웨이브의 생존 적 숫자 감소(UI)
        currentEnemyCount--;

        // 리스트에서 사망하는 적 정보삭제
        enemyList.Remove(enemy);

        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }


    private void SpawnEnemyHPSlider(GameObject enemy) 
    {
        // 적 체력을 나타내는 Slider UI 생성
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        //Sliser UI 오브젝트를 parent("canvas" 오브젝트) 의자식으로 설정
        // Tip.UI 는 캔버스의 자식 오브젝트로 설정되어 있어야 화면엣보인다.
        sliderClone.transform.SetParent(canvasTransform);
        // 꼐층 설정으로 바뀐 크기를 다시 (1,1,1)로 설정 
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }


}
