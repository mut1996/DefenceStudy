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
            ResetToTarget();
        }
    }

    private void ResetToTarget() 
    {
        // �������κ����� �Ÿ��� ���������κ��� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�踦 �̿� 
        // ���� = 
    }
}
