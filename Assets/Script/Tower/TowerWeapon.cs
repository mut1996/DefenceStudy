using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SearchTarget = 0, AttackToTarget}

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;    // �߻�ü ������
    [SerializeField]
    private Transform spawnPoint;       // �߻�ü ���� ��ġ 
    [SerializeField]
    private float attackRate = 0.5f;     // ���ݼӵ�
    [SerializeField]
    private float attackRange = 2.0f;   // ���� ����
    [SerializeField]
    private int attackDamage = 1;       // Ÿ�� ���ݷ� 
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;

    public void SetUp(EnemySpawner enemySpawner) 
    {
        this.enemySpawner = enemySpawner;

        // ���� ���¸� WeaponState.SearchTarget ���� ����
        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState) 
    {
        // ������ ������̴� ���� ����
        StopCoroutine(weaponState.ToString());
        // ���� ����
        weaponState = newState;
        // ���� ���
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget() 
    {
        // �������κ����� �Ÿ��� ���������κ��� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�踦 �̿� 
        // ���� = arctan(y/x)
        // x, y  ������ ���ϱ� 
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // x, y �������� �������� ���� ���ϱ�
        // ������ radion �����̱� ������ Mathf.Rad2Deg�� ���� �� ������ ���� 

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget() 
    {
        while (true)
        {
            // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
            float closestDistSqr = Mathf.Infinity;

            // EnemySpawner �� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
            for (int i = 0; i < enemySpawner.EnemyList.Count; ++i) 
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                // ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
                if (distance <= attackRange && distance <= closestDistSqr) 
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget() 
    {
        while (true)
        {
            //1. target�� �ִ��� �˻�(�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
            if (attackTarget == null) 
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }


            // 2. target�� ���� ���� �ȿ� �ִ��� �˻�(���ݹ����� ����� ���ο� �� Ž��)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange) 
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate �ð� ��ŭ ���
            yield return new WaitForSeconds(attackRate);

            // 4. ���� (�߻�ü ����)
            SpawnProjectile();

        }
    }

    private void SpawnProjectile() 
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ������ �߻�ü���� ���ݴ��(attackTarget) ���� 
        clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
       
    }
}
