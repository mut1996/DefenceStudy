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
        // tip. ���� ü���� damage ��ŭ �����ؼ� ���� ��Ȳ�� �� ���� Ÿ���� ������ ���ÿ� ������
        // enemy.OnDie() �Լ��� ������ ����� �� �ֵ�.

        
        // ���� ������ return 
        if (isDie == true) return;

        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        // ü���� 0 ���� = �� ĳ���� ���
        if (currentHp <= 0) 
        {
            isDie = true;

            // �� ĳ���� ��� 
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // ���� ���� ������  color ������ ���� 
        Color color = spriteRenderer.color;


        // ���� ������ 40%�� ���� 
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 0.05ch ehddks eorl 
        yield return new WaitForSeconds(0.05f);

        // ���� ������ 100% ���� 
        color.a = 1.0f;
        spriteRenderer.color = color;


    }

}
