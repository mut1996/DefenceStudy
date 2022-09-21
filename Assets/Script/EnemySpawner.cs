using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;      // �� ������
    [SerializeField]
    private GameObject enemyHPSliderPrefab; //�� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private Transform canvasTransform;  // UI �� ǥ���ϴ� Canvas ������Ʈ�� Trasform
    [SerializeField]
    private float spawnTime;             // ���� �ð� 
    [SerializeField]
    private Transform[] wayPoints;      // ���� ���������� �̵� ���
    [SerializeField]
    private PlayerHP playerHP;          // �÷��̾� ü�� ������Ʈ 
    private List<Enemy> enemyList;      // ���� �ʿ� �����ϴ� ��� ���� ����

    // ���� ������ ������ EnemySpawner���� �ϱ� ������ set�� �ʿ����
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

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);               // ����Ʈ�� ��� ������ �� ������ ���� 

            SpawnEnemyHPSlider(clone);          // �� ü���� ��Ÿ��s sliderUI ���� �� ���� 

            yield return new WaitForSeconds(spawnTime);
        }
    }



    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy) 
    {

        if (type == EnemyDestroyType.Arrive) 
        {
            playerHP.TakeDamage(1);
        }

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
