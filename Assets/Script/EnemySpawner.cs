using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;      // 적 프리팹
    [SerializeField]
    private float spawnTime;             // 스폰 시간 
    [SerializeField]
    private Transform[] wayPoints;      // 현재 스테이지의 이동 경로
    private List<Enemy> enemyList;      // 현재 맵에 존재하는 모든 적의 정보

    // 적의 생ㄱ성과 삭제는 EnemySpawner에서 하기 때문에 set은 필요없다
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

            enemy.Setup(this,wayPoints);
            enemyList.Add(enemy);               // 리스트에 방금 생성된 적 정보를 저장 

            yield return new WaitForSeconds(spawnTime);
        }
    }



    public void DestroyEnemy(Enemy enemy) 
    {
        // 리스트에서 사망하는 적 정보삭제
        enemyList.Remove(enemy);

        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }



}
