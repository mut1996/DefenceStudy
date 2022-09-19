using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;      // �� ������
    [SerializeField]
    private float spawnTime;             // ���� �ð� 
    [SerializeField]
    private Transform[] wayPoints;      // ���� ���������� �̵� ���
    private List<Enemy> enemyList;      // ���� �ʿ� �����ϴ� ��� ���� ����

    // ���� �������� ������ EnemySpawner���� �ϱ� ������ set�� �ʿ����
    public List<Enemy> EnemyList => enemyList;



    private void Awake()
    {
        enemyList = new List<Enemy>();

        // �� ���� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy() 
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this,wayPoints);
            enemyList.Add(enemy);               // ����Ʈ�� ��� ������ �� ������ ���� 

            yield return new WaitForSeconds(spawnTime);
        }
    }



    public void DestroyEnemy(Enemy enemy) 
    {
        // ����Ʈ���� ����ϴ� �� ��������
        enemyList.Remove(enemy);

        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }



}
