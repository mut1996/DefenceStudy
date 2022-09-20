using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.zero;
    private Transform targetTransform;
    private RectTransform rectTransform;


    public void Setup(Transform target) 
    {
        //Slider UI �� �i�ƴٴ� target ����
        targetTransform = target;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // ���� �ı��Ǿ� �i�ƴٴ� ����� ������� UI �� �ı�;
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // ������Ʈ�� ��ġ�� ���ŵ� ���Ŀ� slider UI �� ���� �Բ� ��ġ�� �����ϵ��� �ϱ�����
        // LateUpdate() ���� ȣ��ȴ�

        // ������Ʈ ���� ��ǥ�� �������� ȭ�鿡���� ��ǥ ���� ���� 
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        // ȭ�鳻���� ��ǥ + distance ��ŭ ������ ��ġ�� sliderUI ��ġ�� ���� 
        rectTransform.position = screenPosition + distance;
    }

}
