using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SearchTarget = 0, AttackToTarget}

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;        // Ÿ�� ����( ���ݷ�, ���ݼӵ��� ()
    [SerializeField]
    private GameObject projectilePrefab;    // �߻�ü ������
    [SerializeField]
    private Transform spawnPoint;       // �߻�ü ���� ��ġ 
   // [SerializeField]
   // private float attackRate = 0.5f;     // ���ݼӵ�
   // [SerializeField]
   // private float attackRange = 2.0f;   // ���� ����
   // [SerializeField]
   // private int attackDamage = 1;       // Ÿ�� ���ݷ� 
    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;

    //public float Damage => attackDamage;
    //public float Rate => attackRate;
    //public float Range => attackRange;
    //public float Level => level + 1;
    public void SetUp(EnemySpawner enemySpawner, PlayerGold playerGold) 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;

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
                if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr) 
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
            if (distance > towerTemplate.weapon[level].range) 
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate �ð� ��ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. ���� (�߻�ü ����)
            SpawnProjectile();

        }
    }

    private void SpawnProjectile() 
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ������ �߻�ü���� ���ݴ��(attackTarget) ���� 
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
       
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

        return true;
    }
}
