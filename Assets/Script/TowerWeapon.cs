using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SearchTarget = 0, AttackToTarget}

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;    // 발사체 프리팹
    [SerializeField]
    private Transform spawnPoint;       // 발사체 생성 위치 
    [SerializeField]
    private float attackRate = 0.5f;     // 공격속도
    [SerializeField]
    private float attackRange = 2.0f;   // 공격 범위
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;

    public void SetUp(EnemySpawner enemySpawner) 
    {
        this.enemySpawner = enemySpawner;

        // 최초 상태를 WeaponState.SearchTarget 으로 설정
        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState) 
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        // 상태 변경
        weaponState = newState;
        // 상태 재생
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
        // 원점으로부터의 거리가 수평축으로부터 각도를 이용해 위치를 구하는 극 좌표계를 이용 
        // 각도 = 
    }
}
