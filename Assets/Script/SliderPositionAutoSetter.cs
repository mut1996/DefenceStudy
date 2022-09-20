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
        //Slider UI 가 쫒아다닐 target 설정
        targetTransform = target;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // 적이 파괴되어 쫒아다닐 대상이 사라지면 UI 도 파괴;
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 오브젝트의 위치가 갱신된 이후에 slider UI 도 같이 함꼐 위치를 설정하도록 하기위해
        // LateUpdate() 에서 호출된다

        // 오브젝트 월드 좌표를 기준으로 화면에서의 좌표 값을 구현 
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        // 화면내에서 좌표 + distance 만큼 떨어진 위치를 sliderUI 위치로 설정 
        rectTransform.position = screenPosition + distance;
    }

}
