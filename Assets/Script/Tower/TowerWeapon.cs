using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType { Cannon = 0, Laser, Slow, Buff}
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towerTemplate;        // Ÿ�� ����( ���ݷ�, ���ݼӵ��� ()
    [SerializeField]
    private Transform spawnPoint;       // �߻�ü ���� ��ġ 
    [SerializeField]
    private WeaponType weaponType;      // ���� �Ӽ� ���� 
    
    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;    // �߻�ü ������

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;      // �������� ���Ǵ� �� (LineRenderer)
    [SerializeField]
    private Transform hitEffect;        //Ÿ�� ȿ��
    [SerializeField]
    private LayerMask targetLayer; // ������ �ε����� ���̾�� 
    
    
    
    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private Tile owenrTile;             // ���� Ÿ���� ��ġ�Ǿ� �ִ� Ÿ�� 

    private float addedDamage;          // ������ �����߰��� ������ 
    private int buffLevel;              // ������ �޴��� ���� ����( 0 : ����, 1~3 �޴� ���� ����)

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

        // ���� ���¸� WeaponState.SearchTarget ���� ����
        //  ChangeState(WeaponState.SearchTarget);
        if (WeaponType == WeaponType.Cannon || WeaponType == WeaponType.Laser) 
        {
            // ���� ���¸� WEaponState.SearchTarget ���� ���� 

            ChangeState(WeaponState.SearchTarget);
        }
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
            /*
            // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
             float closestDistSqr = Mathf.Infinity;

             // EnemySpawner �� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
             for (int i = 0; i < enemySpawner.EnemyList.Count; ++i) 
             {
                 float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                 // ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
                 if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr) 
                 {
                     closestDistSqr = distance;
                     attackTarget = enemySpawner.EnemyList[i].transform;
                 }
             }
             */

            // ���� Ÿ���� ���� �������ִ� ���� ���(�� Ž��)
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
            //1. target�� �ִ��� �˻�(�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
            if (attackTarget == null) 
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }


            // 2. target�� ���� ���� �ȿ� �ִ��� �˻�(���ݹ����� ����� ���ο� �� Ž��)
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

            // 3. attackRate �ð� ��ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. ���� (�߻�ü ����)
            SpawnProjectile();


        }
    }

    private IEnumerator TryAttackLaser() 
    {
        // ������, ������ ȿ�� Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            // target�� �����ϴ°� �������� �˻�
            if (IsPossibleToAttackTarget() == false) 
            {
                // ������, ������ Ÿ��ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ������ ���� 
            SpawnLaser();

            yield return null;
        }


    }

    public void OnBuffAroundTower() 
    {
        // ���� �ʿ� ��ġ�� ��� "Tower" �±� ������Ʈ Ž��
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i) 
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // �̹� ������ �ް��ְ�, ���� ���� Ÿ���� ������Ÿ ���� �����̸� �н� 
            if (weapon.BuffLevel > Level) 
            {
                continue;
            }

            // ���� ���� Ÿ���� �ٸ� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������ 
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range) 
            {
                // ������ ������ ĳ�� ������ Ÿ���̸�
                if (weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser) 
                {
                    // ������ ���� ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // Ÿ���� �ް� �ִ� ���� ���� ���� 
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private Transform FindClosetAttackTarget() 
    {
        // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
        float closestDistSqr = Mathf.Infinity;

        // EnemySpawner �� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
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
        // target �� �ִ��� �˻�(�ٸ� �߻�ü�� ���� ����, Gaol �������� �̵��� ���� �� 
        if (attackTarget == null) 
        {
            return false;
        }

        // target �� ���� ���� �ȿ� �ִ��� �˻�(���� ������ ����� ���ο� �� Ž��)
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

        // ���� �������� ���� ���� ������ ���� �� �� ���� attackTarget�� ������ ������Ʈ�� ����
        for (int i = 0; i < hit.Length; ++i) 
        {
            if (hit[i].transform == attackTarget) 
            {
                // ���� ���� ����
                lineRenderer.SetPosition(0, spawnPoint.position);
                // ���� ��ǥ ����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // Ÿ�� ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;
                // �� ü�� ���� (1�ʿ� damage ����)
                float damage = towerTemplate.weapon[i].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }


    private void SpawnProjectile() 
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ������ �߻�ü���� ���ݴ��(attackTarget) ���� 
        //clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);

       
    }

    public bool Upgrade() 
    {
        // Ÿ�� ���׷��̵忡 �ʟG�� ��尡 ������� �˻�
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost) 
        {
            return false;
        }

        // Ÿ�� ���� ����
        level++;
        // Ÿ�� ���� ����
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // ��� ���� 
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // ���� �Ӽ��� �������̸� 
        if (WeaponType == WeaponType.Laser) 
        {
            // ������ ���� ���⼳�� 
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // Ÿ���� ���׷��̵� �� ��  ��� ���� Ÿ���� ���� ȿ�� ����
        // ���� Ÿ���� ���� Ÿ���� ���, ���� Ÿ���� ���� Ÿ���� ���
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell() 
    {
        // ��� ���� 
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // ���� Ÿ�Ͽ� �ٽ� Ÿ�� �Ǽ� �� ���� �ϵ��� ����
        owenrTile.IsBuildTower = false;
        // Ÿ�� �ı�
        Destroy(gameObject);
    }
}
