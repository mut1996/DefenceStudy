using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab;      // �� ������
    [SerializeField]
    private GameObject enemyHPSliderPrefab; //�� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private Transform canvasTransform;  // UI �� ǥ���ϴ� Canvas ������Ʈ�� Trasform
    //[SerializeField]
    //private float spawnTime;             // ���� �ð� 
    [SerializeField]
    private Transform[] wayPoints;      // ���� ���������� �̵� ���
    [SerializeField]
    private PlayerHP playerHP;          // �÷��̾� ü�� ������Ʈ 
    [SerializeField]
    private PlayerGold playerGold;      // �÷��̾� ��� ������Ʈ 
    private Wave currentWave;           // ���� ���̺� ���� 
    private int currentEnemyCount;      // ���� ���̺꿡 �����ִ� �� ����( ���̺� ���۽� max �μ���, �� ����� -1)
    private List<Enemy> enemyList;      // ���� �ʿ� �����ϴ� ��� ���� ����


    // ���� ������ ������ EnemySpawner���� �ϱ� ������ set�� �ʿ����
    public List<Enemy> EnemyList => enemyList;

    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;


    private void Awake()
    {
        enemyList = new List<Enemy>();

        // �� ���� �ڷ�ƾ �Լ� ȣ��
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave) 
    {
        // �Ű� ������ �޾ƿ� ���̺� ��������
        currentWave = wave;
        // ���� ���̺��� �ִ� �� ���ڸ� ����
        currentEnemyCount = currentWave.maxEnemyCount;
        // ���� ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy() 
    {
        // ������̺꿡 �������� ��    
        int spawnEnemyCount = 0;


        //while (true)
        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // GameObject clone = Instantiate(enemyPrefab);
            // ���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϵ��� �����ϰ�, �� ������Ʈ ����
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);               // ����Ʈ�� ��� ������ �� ������ ���� 

            SpawnEnemyHPSlider(clone);          // �� ü���� ��Ÿ��s sliderUI ���� �� ���� 

            // ���� ���̺꿡�� ������ ���� ���� + 1 
            spawnEnemyCount++;

            // �� �����̺� ���� spawnTime �� �ٸ� �� �ֱ� ������ ���� ���̺�(currentWave) �� spawnTIme ��� 
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }



    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold) 
    {

        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }

        // ���� �÷��̾��� �߻�ü���� ��� ���� ��
        else if (type == EnemyDestroyType.Kill)
        {
            // ���� ������ ���� ����� ��� ȹ�� 
            playerGold.CurrentGold += gold;
        }
        // ���� ����� ������ ���� ���̺��� ���� �� ���� ����(UI)
        currentEnemyCount--;

        // ����Ʈ���� ����ϴ� �� ��������
        enemyList.Remove(enemy);

        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }


    private void SpawnEnemyHPSlider(GameObject enemy) 
    {
        // �� ü���� ��Ÿ���� Slider UI ����
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        //Sliser UI ������Ʈ�� parent("canvas" ������Ʈ) ���ڽ����� ����
        // Tip.UI �� ĵ������ �ڽ� ������Ʈ�� �����Ǿ� �־�� ȭ�鿧���δ�.
        sliderClone.transform.SetParent(canvasTransform);
        // ���� �������� �ٲ� ũ�⸦ �ٽ� (1,1,1)�� ���� 
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }


}
