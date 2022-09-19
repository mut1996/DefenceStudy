using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int wayPointCount;       // �̵� ��� ����
    private Transform[] wayPoints;    // �̵� ��� ����
    private int currentIndex = 0;    // ���� ��ǥ���� �ε���
    private Movement2D movement2D;
    private EnemySpawner enemySpawner;  // ���� ������ ������ ���� �ʰ� EnemySpawner�� �˷��� ���� 

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // ���� ���� 
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;


        // ���� ��ġ�� ù���� wayPoint ��ġ�� ����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵�/ ��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");

    }

    private IEnumerator OnMove() 
    {
        // ���� �̵� ���� ���� 
        NextMoveTo();

        while (true)
        {
            // �� ������Ʈ ȸ��
            transform.Rotate(Vector3.forward * 10);

            // ���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02 movement2D.MoveSpeed ���� ���� �� if ���ǹ��� ����
            // Tip. movement2D.MoveSpeed �� �����ִ� ������ �ӵ��� ������ �� �����ӿ� 0.02 ���� ũ�� �����̱� ������ 
            // if ���ǹ��� �ɸ��� �ʰ� ��θ� Ż���ϴ� ������Ʈ�� �߻��Ҽ� �ִ�. 

            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed) 
            {
                // ���� �̵� ���� ���� 
                NextMoveTo();
            }
       
            yield return null;
        }

    }

    // �̵����� ���� �Լ�
    private void NextMoveTo() 
    {
        // ���� �̵��� wayPoints �� �����ֵ���
        if (currentIndex < wayPointCount - 1)
        {
            // �� ��ġ�� ��Ȯ�ϰ� ��ǥ ��ġ�� ���� 
            transform.position = wayPoints[currentIndex].position;
            // �̵� ���� ���� => ���� ��ǥ����(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }

        else 
        {
            // �� ������Ʈ ����
            //Destroy(gameObject);

            OnDie();
        }
    }


    public void OnDie() 
    {
        // EnemySpawner���� ����Ʈ�� �� ������ �����ϱ� ������ DSestroy()�� �������� �ʰ�
        enemySpawner.DestroyEnemy(this);
    }
}
