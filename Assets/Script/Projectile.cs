using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;

    public void Setup(Transform target) 
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;
    }

    private void Update()
    {
        if (target != null)
        {
            // �߻�ü�� traget �� ��ġ�� �̵�
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else        // target �� ������� 
        {
            // �߻�ü ������Ʈ ����
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;         // ���� �ƴ� ���� �ε�����
        if (collision.transform != target) return;          // ���� targetd �� ���� �ƴҶ�

        collision.GetComponent<Enemy>().OnDie();    // �� ����Լ� ȣ��
        Destroy(gameObject);                        // �߻�ü ������Ʈ ����
    }

}
