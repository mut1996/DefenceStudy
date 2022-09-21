using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;      // 적 프리팹
    [SerializeField]
    private GameObject enemyHPSliderPrefab; //적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;  // UI 를 표현하는 Canvas 오브젝트의 Trasform
    [SerializeField]
    private float spawnTime;             // 스폰 시간 
    [SerializeField]
    private Transform[] wayPoints;      // 현재 스테이지의 이동 경로
    [SerializeField]
    private PlayerHP playerHP;          // 플레이어 체력 컴포넌트 
    private List<Enemy> enemyList;      // 현재 맵에 존재하는 모든 적의 정보

    // 적의 생성과 삭제는 EnemySpawner에서 하기 때문에 set은 필요없다
    public List<Enemy> EnemyList => enemyList;



    private void Awake()
    {
        enemyList = new List<Enemy>();

        // 적 생성 코루틴 함수 호출
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy() 
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);               // 리스트에 방금 생성된 적 정보를 저장 

            SpawnEnemyHPSlider(clone);          // 적 체력을 나타는s sliderUI 생성 및 설정 

            yield return new WaitForSeconds(spawnTime);
        }
    }



    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy) 
    {

        if (type == EnemyDestroyType.Arrive) 
        {
            playerHP.TakeDamage(1);
        }

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
