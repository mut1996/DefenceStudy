using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType { Cannon = 0, Laser, Slow, Buff}
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towerTemplate;        // 타워 정보( 공격력, 공격속도등 ()
    [SerializeField]
    private Transform spawnPoint;       // 발사체 생성 위치 
    [SerializeField]
    private WeaponType weaponType;      // 무기 속성 설정 
    
    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;    // 발사체 프리팹

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;      // 레이저로 사용되는 선 (LineRenderer)
    [SerializeField]
    private Transform hitEffect;        //타격 효과
    [SerializeField]
    private LayerMask targetLayer; // 광선이 부딪히는 레이어설정 
    
    
    
    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private Tile owenrTile;             // 현재 타워가 배치되어 있는 타일 

    private float addedDamage;          // 버프에 의해추가된 데미지 
    private int buffLevel;              // 버프릴 받는지 여부 설정( 0 : 버프, 1~3 받는 버프 레벨)

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level + 1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;

    public float AddedDamage 
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }

    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }




    //public float Damage => attackDamage;
    //public float Rate => attackRate;
    //public float Range => attackRange;
    //public float Level => level + 1;
    public void SetUp(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile) 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.owenrTile = ownerTile;

        // 최초 상태를 WeaponState.SearchTarget 으로 설정
        //  ChangeState(WeaponState.SearchTarget);
        if (WeaponType == WeaponType.Cannon || WeaponType == WeaponType.Laser) 
        {
            // 최초 상태를 WEaponState.SearchTarget 으로 설정 

            ChangeState(WeaponState.SearchTarget);
        }
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
            RotateToTarget();
        }
    }

    private void RotateToTarget() 
    {
        // 원점으로부터의 거리가 수평축으로부터 각도를 이용해 위치를 구하는 극 좌표계를 이용 
        // 각도 = arctan(y/x)
        // x, y  변위값 구하기 
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radion 단위이기 떄문에 Mathf.Rad2Deg를 곱해 도 단위를 구함 

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget() 
    {
        while (true)
        {
            /*
            // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
             float closestDistSqr = Mathf.Infinity;

             // EnemySpawner 의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
             for (int i = 0; i < enemySpawner.EnemyList.Count; ++i) 
             {
                 float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                 // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
                 if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr) 
                 {
                     closestDistSqr = distance;
                     attackTarget = enemySpawner.EnemyList[i].transform;
                 }
             }
             */

            // 현재 타워에 가장 가까이있는 공격 대상(적 탐색)
            attackTarget = FindClosetAttackTarget();

            if (attackTarget != null) 
            {
                if (WeaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (WeaponType == WeaponType.Laser) 
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }


         

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon() 
    {
        while (true)
        {
            /*
            //1. target이 있는지 검사(다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
            if (attackTarget == null) 
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }


            // 2. target이 공격 번위 안에 있는지 검사(공격범위를 벗어나면 새로운 적 탐색)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > towerTemplate.weapon[level].range) 
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

           
             */

            if (IsPossibleToAttackTarget() == false) 
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate 시간 만큼 대기
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. 공격 (발사체 생성)
            SpawnProjectile();


        }
    }

    private IEnumerator TryAttackLaser() 
    {
        // 레이저, 레이저 효과 활성화
        EnableLaser();

        while (true)
        {
            // target을 공격하는게 간으한지 검사
            if (IsPossibleToAttackTarget() == false) 
            {
                // 레이저, 레이저 타격효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이저 공격 
            SpawnLaser();

            yield return null;
        }


    }

    public void OnBuffAroundTower() 
    {
        // 현재 맵에 배치된 모든 "Tower" 태그 오브젝트 탐색
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i) 
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // 이미 버프를 받고있고, 현재 버프 타워의 레벨보타 높은 버프이면 패스 
            if (weapon.BuffLevel > Level) 
            {
                continue;
            }

            // 현재 버프 타워와 다른 타워의 거리를 검사해서 범위 안에 타워가 있으면 
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range) 
            {
                // 공격이 가능한 캐논 레이저 타입이면
                if (weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser) 
                {
                    // 버프에 의해 공격력 증가
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // 타워가 받고 있는 버프 레벨 설정 
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private Transform FindClosetAttackTarget() 
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closestDistSqr = Mathf.Infinity;

        // EnemySpawner 의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;

    }

    private bool IsPossibleToAttackTarget() 
    {
        // target 이 있는지 검사(다른 발사체에 의해 제거, Gaol 지점까지 이동해 삭제 등 
        if (attackTarget == null) 
        {
            return false;
        }

        // target 이 공격 범위 안에 있는지 검사(공격 범위를 버어나면 새로운 적 탐색)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range) 
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void EnableLaser() 
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser() 
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);

    }

    private void SpawnLaser() 
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction,
                                                    towerTemplate.weapon[level].range, targetLayer);

        // 같은 방향으로 여러 개의 광선을 쏴서 그 중 현재 attackTarget과 동일한 오브젝트를 검출
        for (int i = 0; i < hit.Length; ++i) 
        {
            if (hit[i].transform == attackTarget) 
            {
                // 선의 시작 지점
                lineRenderer.SetPosition(0, spawnPoint.position);
                // 선의 목표 지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;
                // 적 체력 감소 (1초에 damage 감소)
                float damage = towerTemplate.weapon[i].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }


    private void SpawnProjectile() 
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // 생성한 발사체에게 공격대상(attackTarget) 제공 
        //clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);

       
    }

    public bool Upgrade() 
    {
        // 타워 업그레이드에 필욯나 골드가 충분한지 검사
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost) 
        {
            return false;
        }

        // 타워 레벨 증가
        level++;
        // 타워 외형 변경
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // 골드 차감 
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // 무기 속성이 레이저이면 
        if (WeaponType == WeaponType.Laser) 
        {
            // 레벨에 따라 굵기설정 
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // 타워가 업그레이드 될 떄  모든 버프 타워의 버프 효과 갱신
        // 현재 타워가 버프 타워인 경우, 현재 타워가 공격 타워인 경우
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell() 
    {
        // 골드 증가 
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // 현재 타일에 다시 타워 건설 이 가능 하도록 설정
        owenrTile.IsBuildTower = false;
        // 타워 파괴
        Destroy(gameObject);
    }
}
