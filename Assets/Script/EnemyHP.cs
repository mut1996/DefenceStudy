using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP;
    private float currentHp;
    private bool isDie = false;
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHp => maxHP;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        currentHp = maxHP;
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage) 
    {
        // tip. 적의 체력이 damage 만큼 감소해서 죽을 상황일 때 여러 타워의 공격을 동시에 받으면
        // enemy.OnDie() 함수가 여러번 실행될 수 있따.

        
        // 적이 죽으면 return 
        if (isDie == true) return;

        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        // 체력이 0 이하 = 적 캐릭터 사망
        if (currentHp <= 0) 
        {
            isDie = true;

            // 적 캐릭터 사망 
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 현재 색의 색상을  color 변수에 전장 
        Color color = spriteRenderer.color;


        // 적의 투명도를 40%로 설정 
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 0.05ch ehddks eorl 
        yield return new WaitForSeconds(0.05f);

        // 적의 투명도를 100% 설정 
        color.a = 1.0f;
        spriteRenderer.color = color;


    }

}
