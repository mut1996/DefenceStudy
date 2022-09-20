using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;
    private int damage;

    public void Setup(Transform target, int damage) 
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;                           // 타워가 설정해준 target
        this.damage = damage;                           // 타워가 설정해준 공격력
    }

    private void Update()
    {
        if (target != null)
        {
            // 발사체를 traget 의 위치로 이동
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else        // target 이 사라지면 
        {
            // 발사체 오브젝트 삭제
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;         // 적이 아닌 대상과 부딪히면
        if (collision.transform != target) return;          // 현재 targetd 인 적이 아닐때


        //collision.GetComponent<Enemy>().OnDie();    // 적 사망함수 호출

        collision.GetComponent<EnemyHP>().TakeDamage(damage);    // 적체력을 damage 만큼 감소
        Destroy(gameObject);                                    // 발사체 오브젝트 삭제
    }

}
